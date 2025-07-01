using Contract.DTOs;
using Microsoft.AspNetCore.Http;
using Repository.Interfaces;
using Repository.Models;
using Service.Interfaces;
using System.Text.Json;

public class RoutineService : IRoutineService
{
    private readonly IOpenAiVisionService _visionService;
    private readonly IProductRepository _productRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IRoutineRepository _routineRepo;

    public RoutineService(
        IOpenAiVisionService visionService,
        IProductRepository productRepo,
        ICategoryRepository categoryRepo,
        IRoutineRepository routineRepo)
    {
        _visionService = visionService;
        _productRepo = productRepo;
        _categoryRepo = categoryRepo;
        _routineRepo = routineRepo;
    }

    public async Task<RoutineResultDto> AnalyzeAndCreateRoutineAsync(Guid userId, IFormFile image)
    {
        if (image == null || image.Length == 0)
            throw new Exception("Chưa upload file.");

        var oldRoutine = await _routineRepo.GetByUserIdAsync(userId);
        if (oldRoutine != null)
        {
            await _routineRepo.DeleteRoutineProductChecksByRoutineIdAsync(oldRoutine.Id);
            await _routineRepo.DeleteAsync(oldRoutine);
        }

        byte[] imageBytes;
        using (var ms = new MemoryStream())
        {
            await image.CopyToAsync(ms);
            imageBytes = ms.ToArray();
        }
        var products = await _productRepo.GetAllAsync();
        var result = await _visionService.AnalyzeFaceAndSuggestRoutineAsync(imageBytes, products.Select(p => p.Name).ToList());

        if (result.Trim().ToLower() == "ảnh không hợp lệ")
            throw new Exception("Ảnh không hợp lệ");

        var jsonStart = result.IndexOf('{');
        var jsonEnd = result.LastIndexOf('}');
        if (jsonStart < 0 || jsonEnd < 0 || jsonEnd <= jsonStart)
            throw new Exception("Phân tích ảnh thất bại! OpenAI không trả về JSON hợp lệ:\n" + result);

        var json = result.Substring(jsonStart, jsonEnd - jsonStart + 1);
        var doc = JsonDocument.Parse(json);


        List<RoutineProductDto> Map(string prop)
        {
            return doc.RootElement.GetProperty(prop).EnumerateArray()
                .Select(name =>
                {
                    var p = products.FirstOrDefault(x => x.Name.Equals(name.GetString(), StringComparison.OrdinalIgnoreCase));
                    if (p == null) return null;

                    // Lấy category (nên await nếu repo là async)
                    var catList = _productRepo.GetCategoriesByProductIdAsync(p.Id).GetAwaiter().GetResult();
                    return new RoutineProductDto
                    {
                        ProductId = p.Id,
                        Name = p.Name,
                        ImageLink = p.ImageLink,
                        Categories = catList.Select(c => c.Name).ToList()
                    };
                })
                .Where(x => x != null)
                .ToList();
        }


        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(29);
        var routine = new DailyRoutine
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoutineDetails = json,
            StartDate = startDate,
            EndDate = endDate,
            CreatedAt = DateTime.UtcNow,
        };
        await _routineRepo.AddAsync(routine);

        return new RoutineResultDto
        {
            RoutineId = routine.Id,
            StartDate = startDate,
            EndDate = endDate,
            Morning = Map("morning"),
            Noon = Map("noon"),
            Night = Map("night"),
            Advice = doc.RootElement.GetProperty("advice").GetString()
        };
    }

    public async Task<RoutineResultDto> GetRoutineAsync(Guid userId)
    {
        var routine = await _routineRepo.GetByUserIdAsync(userId);
        if (routine == null) return null;

        //lay ds product
        var products = await _productRepo.GetAllAsync();

        var doc = JsonDocument.Parse(routine.RoutineDetails);

        List<RoutineProductDto> Map(string prop)
        {
            return doc.RootElement.GetProperty(prop).EnumerateArray()
                .Select(name =>
                {
                    var p = products.FirstOrDefault(x => x.Name.Equals(name.GetString(), StringComparison.OrdinalIgnoreCase));
                    if (p == null) return null;

                    var catList = _productRepo.GetCategoriesByProductIdAsync(p.Id).GetAwaiter().GetResult();
                    return new RoutineProductDto
                    {
                        ProductId = p.Id,
                        Name = p.Name,
                        ImageLink = p.ImageLink,
                        Categories = catList.Select(c => c.Name).ToList()
                    };
                })
                .Where(x => x != null)
                .ToList();
        }

        return new RoutineResultDto
        {
            RoutineId = routine.Id,
            StartDate = (DateTime)routine.StartDate,
            EndDate = (DateTime)routine.EndDate,
            Morning = Map("morning"),
            Noon = Map("noon"),
            Night = Map("night"),
            Advice = doc.RootElement.TryGetProperty("advice", out var ad) ? ad.GetString() : ""
        };
    }

    public async Task CheckRoutineProductAsync(CheckRoutineProductDto dto, Guid userId)
    {
        var sessionStr = dto.Session.ToString().ToLower();
        var check = new RoutineProductCheck
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoutineId = dto.RoutineId,
            ProductId = dto.ProductId,
            Session = sessionStr,
            UsageDate = dto.UsageDate.Date,
            IsChecked = dto.IsChecked,
            CreatedAt = DateTime.UtcNow
        };
        await _routineRepo.UpsertCheckAsync(check);
    }

    public async Task<List<RoutineCheckHistoryDto>> GetWeeklyProgressAsync(Guid userId)
    {
        var routine = await _routineRepo.GetByUserIdAsync(userId);
        if (routine == null) return new List<RoutineCheckHistoryDto>();

        var (weekStart, weekEnd) = GetCurrentWeekRange(DateTime.UtcNow);

        var checks = await _routineRepo.GetCheckHistoryAsync(routine.Id, weekStart, weekEnd);

        var daily = new List<RoutineCheckHistoryDto>();
        for (var d = weekStart; d <= weekEnd; d = d.AddDays(1))
        {
            var items = checks.Where(x => x.UsageDate.Date == d.Date).ToList();
            daily.Add(new RoutineCheckHistoryDto
            {
                UsageDate = d,
                Total = 6, // 2x3 session
                Checked = items.Count(x => x.IsChecked),
                Percent = (int)Math.Round(items.Count(x => x.IsChecked) * 100.0 / 6)
            });
        }
        return daily;
    }

    private static (DateTime weekStart, DateTime weekEnd) GetCurrentWeekRange(DateTime now)
    {
        int delta = (now.DayOfWeek == DayOfWeek.Sunday) ? -6 : (DayOfWeek.Monday - now.DayOfWeek);
        var weekStart = now.Date.AddDays(delta);
        var weekEnd = weekStart.AddDays(6);
        return (weekStart, weekEnd);
    }
    public async Task<RoutineDailyResultDto> GetRoutineDailyAsync(Guid userId, DateTime? date = null)
    {
        var routine = await _routineRepo.GetByUserIdAsync(userId);
        if (routine == null) return null;

        var usageDate = (date ?? DateTime.UtcNow.Date).Date;
        var products = await _productRepo.GetAllAsync();

        var doc = JsonDocument.Parse(routine.RoutineDetails);

        var checks = await _routineRepo.GetChecksByRoutineAndDateAsync(routine.Id, usageDate);

        List<RoutineDailyProductDto> Map(string session)
        {
            return doc.RootElement.GetProperty(session).EnumerateArray()
                .Select(name =>
                {
                    var p = products.FirstOrDefault(x => x.Name.Equals(name.GetString(), StringComparison.OrdinalIgnoreCase));
                    if (p == null) return null;
                    var check = checks.FirstOrDefault(x => x.ProductId == p.Id && x.Session == session);

                    var catList = _productRepo.GetCategoriesByProductIdAsync(p.Id).GetAwaiter().GetResult();
                    return new RoutineDailyProductDto
                    {
                        ProductId = p.Id,
                        Name = p.Name,
                        Session = session,
                        IsChecked = check?.IsChecked ?? false,
                        ImageLink = p.ImageLink,
                        Categories = catList.Select(c => c.Name).ToList()
                    };
                })
                .Where(x => x != null)
                .ToList();
        }

        var morning = Map("morning");
        var noon = Map("noon");
        var night = Map("night");

        var total = morning.Count + noon.Count + night.Count;
        var checkedCount = morning.Count(x => x.IsChecked) + noon.Count(x => x.IsChecked) + night.Count(x => x.IsChecked);
        var percent = total > 0 ? (int)Math.Round(checkedCount * 100.0 / total) : 0;

        return new RoutineDailyResultDto
        {
            UsageDate = usageDate,
            Morning = morning,
            Noon = noon,
            Night = night,
            Total = total,
            Checked = checkedCount,
            Percent = percent
        };
    }

}

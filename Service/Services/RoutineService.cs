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
    private readonly IDailyRoutineRepository _routineRepo;

    public RoutineService(
        IOpenAiVisionService visionService,
        IProductRepository productRepo,
        ICategoryRepository categoryRepo,
        IDailyRoutineRepository routineRepo)
    {
        _visionService = visionService;
        _productRepo = productRepo;
        _categoryRepo = categoryRepo;
        _routineRepo = routineRepo;
    }

    public async Task<RoutineResultDto> AnalyzeAndSaveRoutineAsync(Guid userId, IFormFile image)
    {
        if (image == null || image.Length == 0)
            throw new Exception("Chưa upload file.");

        //check routine cu~
        var oldRoutine = await _routineRepo.GetByUserIdAsync(userId);
        if (oldRoutine != null)
        {
            await _routineRepo.DeleteRoutineProductsByRoutineIdAsync(oldRoutine.Id);
            await _routineRepo.DeleteAsync(oldRoutine);
        }

        //image -> bytes
        byte[] imageBytes;
        using (var ms = new MemoryStream())
        {
            await image.CopyToAsync(ms);
            imageBytes = ms.ToArray();
        }

        //get list product/cate
        var products = await _productRepo.GetAllAsync();
        var allCategories = await _categoryRepo.GetAllAsync();

        //send cho ai
        var result = await _visionService.AnalyzeFaceAndSuggestRoutineAsync(
            imageBytes,
            products.Select(p => p.Name).ToList()
        );

        //vi du anh ko hop le thi throw loi
        if (result.Trim().ToLower() == "ảnh không hợp lệ")
            throw new Exception("Ảnh không hợp lệ");

        //parse json result
        RoutineResultDto routine;
        try
        {
            var jsonStart = result.IndexOf('{');
            var jsonEnd = result.LastIndexOf('}');
            var json = result.Substring(jsonStart, jsonEnd - jsonStart + 1);

            var doc = JsonDocument.Parse(json);
            var morningNames = doc.RootElement.GetProperty("morning").EnumerateArray().Select(x => x.GetString()).ToList();
            var noonNames = doc.RootElement.GetProperty("noon").EnumerateArray().Select(x => x.GetString()).ToList();
            var nightNames = doc.RootElement.GetProperty("night").EnumerateArray().Select(x => x.GetString()).ToList();
            var advice = doc.RootElement.TryGetProperty("advice", out var adviceProp)
                ? adviceProp.GetString()
                : null;

            routine = new RoutineResultDto
            {
                Morning = MapProductsWithCategory(morningNames, products, _productRepo),
                Noon = MapProductsWithCategory(noonNames, products, _productRepo),
                Night = MapProductsWithCategory(nightNames, products, _productRepo),
                Advice = advice // GÁN THÊM DÒNG NÀY
            };
        }
        catch
        {
            throw new Exception("OpenAI trả về dữ liệu không đúng format.");
        }

        //luu vao` db
        var routineEntity = new DailyRoutine
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoutineDetails = JsonSerializer.Serialize(routine),
            CreatedAt = DateTime.UtcNow,
        };
        await _routineRepo.AddAsync(routineEntity);

        //luu mapping (cai nay quan trong nhat, xem ki~ gium em vi loi mapping la loi het)
        var allProducts = routine.Morning.Concat(routine.Noon).Concat(routine.Night).ToList();
        foreach (var prod in allProducts)
        {
            await _routineRepo.AddRoutineProductAsync(new DailyRoutineProduct
            {
                Id = Guid.NewGuid(),
                DailyRoutineId = routineEntity.Id,
                ProductId = prod.Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        return routine;
    }

    //dung mapping lay dung category
    private List<ProductDto> MapProductsWithCategory(
        List<string> productNames,
        List<SuggestedProduct> allProducts,
        IProductRepository productRepo)
    {
        var result = new List<ProductDto>();
        foreach (var n in productNames)
        {
            var product = allProducts.FirstOrDefault(p => p.Name.Equals(n, StringComparison.OrdinalIgnoreCase))
                ?? throw new Exception($"Sản phẩm '{n}' không tồn tại");

   
            var cats = productRepo.GetCategoriesByProductIdAsync(product.Id).GetAwaiter().GetResult();

            result.Add(new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ProductLink = product.ProductLink,
                ImageLink = product.ImageLink,
                Categories = cats.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                }).ToList()
            });
        }
        return result;
    }

    public async Task<RoutineResultFullDto> GetRoutineByUserIdAsync(Guid userId)
    {
        var routineEntity = await _routineRepo.GetByUserIdAsync(userId);
        if (routineEntity == null) return null;

        //parse detail cua routine
        RoutineResultFullDto result = new RoutineResultFullDto();
        if (!string.IsNullOrEmpty(routineEntity.RoutineDetails))
        {
            //dua vao format trong db: { "Morning": [...], "Noon": [...], "Night": [...], "Advice": "..." }
            using var doc = JsonDocument.Parse(routineEntity.RoutineDetails);

            result.Morning = doc.RootElement.TryGetProperty("Morning", out var m)
                ? JsonSerializer.Deserialize<List<ProductDto>>(m.GetRawText()) : new List<ProductDto>();
            result.Noon = doc.RootElement.TryGetProperty("Noon", out var n)
                ? JsonSerializer.Deserialize<List<ProductDto>>(n.GetRawText()) : new List<ProductDto>();
            result.Night = doc.RootElement.TryGetProperty("Night", out var ni)
                ? JsonSerializer.Deserialize<List<ProductDto>>(ni.GetRawText()) : new List<ProductDto>();
            result.Advice = doc.RootElement.TryGetProperty("Advice", out var ad) ? ad.GetString() : "";
        }
        result.CreatedAt = routineEntity.CreatedAt;
        return result;
    }



}

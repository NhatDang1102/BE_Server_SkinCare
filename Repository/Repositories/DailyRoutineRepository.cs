using Contract.DTOs;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class RoutineRepository : IRoutineRepository
{
    private readonly SkinCareAppContext _ctx;
    public RoutineRepository(SkinCareAppContext ctx) { _ctx = ctx; }

    public async Task<DailyRoutine> GetByUserIdAsync(Guid userId) =>
        await _ctx.DailyRoutines.FirstOrDefaultAsync(r => r.UserId == userId && r.EndDate >= DateTime.UtcNow);

    public async Task<DailyRoutine> GetByIdAsync(Guid routineId) =>
        await _ctx.DailyRoutines.FirstOrDefaultAsync(r => r.Id == routineId);

    public async Task AddAsync(DailyRoutine routine)
    {
        _ctx.DailyRoutines.Add(routine);
        await _ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(DailyRoutine routine)
    {
        _ctx.DailyRoutines.Remove(routine);
        await _ctx.SaveChangesAsync();
    }

    public async Task<RoutineProductCheck> GetCheckAsync(Guid userId, Guid routineId, Guid productId, string session, DateTime date)
    {
        return await _ctx.RoutineProductChecks.FirstOrDefaultAsync(
            x => x.UserId == userId && x.RoutineId == routineId && x.ProductId == productId
                && x.Session == session && x.UsageDate == date.Date);
    }

    public async Task UpsertCheckAsync(RoutineProductCheck check)
    {
        var exist = await _ctx.RoutineProductChecks.FirstOrDefaultAsync(
            x => x.UserId == check.UserId && x.RoutineId == check.RoutineId
                && x.ProductId == check.ProductId && x.Session == check.Session && x.UsageDate == check.UsageDate);
        if (exist != null)
        {
            exist.IsChecked = check.IsChecked;
        }
        else
        {
            _ctx.RoutineProductChecks.Add(check);
        }
        await _ctx.SaveChangesAsync();
    }

    public async Task<List<RoutineProductCheck>> GetChecksByRoutineAndDateAsync(Guid routineId, DateTime date)
    {
        return await _ctx.RoutineProductChecks
            .Where(x => x.RoutineId == routineId && x.UsageDate == date.Date)
            .ToListAsync();
    }

    public async Task<List<RoutineProductCheck>> GetCheckHistoryAsync(Guid routineId, DateTime weekStart, DateTime weekEnd)
    {
        return await _ctx.RoutineProductChecks
            .Where(x => x.RoutineId == routineId
                && x.UsageDate >= weekStart
                && x.UsageDate <= weekEnd)
            .ToListAsync();
    }
    public async Task DeleteRoutineProductChecksByRoutineIdAsync(Guid routineId)
    {
        var checks = _ctx.RoutineProductChecks.Where(x => x.RoutineId == routineId);
        _ctx.RoutineProductChecks.RemoveRange(checks);
        await _ctx.SaveChangesAsync();
    }

    public async Task AddAsync(RoutineFeedback feedback)
    {
        _ctx.RoutineFeedback.Add(feedback);
        await _ctx.SaveChangesAsync();
    }

    public async Task<List<RoutineFeedbackAdminDto>> GetAllFeedbacksAsync()
    {
        return await (from fb in _ctx.RoutineFeedback
                      join u in _ctx.Users on fb.UserId equals u.Id
                      select new RoutineFeedbackAdminDto
                      {
                          Id = fb.Id,
                          UserEmail = u.Email ?? "",
                          UserName = u.Name ?? "",
                          RoutineId = fb.RoutineId,
                          Message = fb.Message ?? "",
                          ImageUrl = fb.ImageUrl ?? "",   
                          CreatedAt = fb.CreatedAt
                      })
                      .OrderByDescending(x => x.CreatedAt)
                      .ToListAsync();
    }


}

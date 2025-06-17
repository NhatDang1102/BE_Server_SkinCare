using Contract.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRoutineService
{
    Task<RoutineResultDto> AnalyzeAndCreateRoutineAsync(Guid userId, IFormFile image);
    Task<RoutineResultDto> GetRoutineAsync(Guid userId);
    Task CheckRoutineProductAsync(CheckRoutineProductDto dto, Guid userId);
    Task<List<RoutineCheckHistoryDto>> GetWeeklyProgressAsync(Guid userId);
    Task<RoutineDailyResultDto> GetRoutineDailyAsync(Guid userId, DateTime? date = null);

}

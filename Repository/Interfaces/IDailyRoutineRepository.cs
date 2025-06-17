using Repository.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRoutineRepository
{
    Task<DailyRoutine> GetByUserIdAsync(Guid userId);
    Task<DailyRoutine> GetByIdAsync(Guid routineId);
    Task AddAsync(DailyRoutine routine);
    Task DeleteAsync(DailyRoutine routine);

    // Product check CRUD
    Task<RoutineProductCheck> GetCheckAsync(Guid userId, Guid routineId, Guid productId, string session, DateTime date);
    Task UpsertCheckAsync(RoutineProductCheck check); // insert/update
    Task<List<RoutineProductCheck>> GetChecksByRoutineAndDateAsync(Guid routineId, DateTime date);
    Task<List<RoutineProductCheck>> GetCheckHistoryAsync(Guid routineId, DateTime startDate, DateTime endDate);
    Task DeleteRoutineProductChecksByRoutineIdAsync(Guid routineId);
}

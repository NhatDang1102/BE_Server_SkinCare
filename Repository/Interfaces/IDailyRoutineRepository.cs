using Repository.Models;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IDailyRoutineRepository
    {
        Task AddAsync(DailyRoutine routine);
        Task AddRoutineProductAsync(DailyRoutineProduct mapping);
        Task<DailyRoutine> GetByUserIdAsync(Guid userId);
        Task DeleteRoutineProductsByRoutineIdAsync(Guid routineId);
        Task DeleteAsync(DailyRoutine routine);
    }
}

using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Repository.Models;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class DailyRoutineRepository : IDailyRoutineRepository
    {
        private readonly SkinCareAppContext _ctx;
        public DailyRoutineRepository(SkinCareAppContext ctx) { _ctx = ctx; }

        public async Task AddAsync(DailyRoutine routine)
        {
            _ctx.DailyRoutines.Add(routine);
            await _ctx.SaveChangesAsync();
        }

        public async Task AddRoutineProductAsync(DailyRoutineProduct mapping)
        {
            _ctx.DailyRoutineProducts.Add(mapping);
            await _ctx.SaveChangesAsync();
        }

        public async Task<DailyRoutine> GetByUserIdAsync(Guid userId)
        {
            return await _ctx.DailyRoutines.FirstOrDefaultAsync(r => r.UserId == userId);
        }

        public async Task DeleteRoutineProductsByRoutineIdAsync(Guid routineId)
        {
            var mappings = _ctx.DailyRoutineProducts.Where(m => m.DailyRoutineId == routineId);
            _ctx.DailyRoutineProducts.RemoveRange(mappings);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(DailyRoutine routine)
        {
            _ctx.DailyRoutines.Remove(routine);
            await _ctx.SaveChangesAsync();
        }
    }
}

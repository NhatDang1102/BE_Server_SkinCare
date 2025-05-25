using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ISuggestedProductRepository
    {
        Task<List<SuggestedProduct>> GetAllAsync();
        Task<SuggestedProduct> GetByIdAsync(Guid id);
        Task AddAsync(SuggestedProduct product);
        Task UpdateAsync(SuggestedProduct product);
        Task DeleteAsync(SuggestedProduct product);
    }
}

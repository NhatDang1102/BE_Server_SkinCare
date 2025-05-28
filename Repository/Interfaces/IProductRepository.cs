using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Models;

namespace Repository.Interfaces
{
    public interface IProductRepository
    {
        Task<List<SuggestedProduct>> GetAllAsync();
        Task<SuggestedProduct> GetByIdAsync(Guid id);
        Task<List<ProductCategory>> GetCategoriesByProductIdAsync(Guid productId);
        Task AddAsync(SuggestedProduct product, List<Guid> categoryIds);
        Task UpdateAsync(SuggestedProduct product, List<Guid> categoryIds);
        Task DeleteAsync(SuggestedProduct product);
    }

}

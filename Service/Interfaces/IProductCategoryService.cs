using Contract.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IProductCategoryService
    {
        Task<IEnumerable<ProductCategoryResponse>> GetAllAsync();
        Task<ProductCategoryResponse> GetByIdAsync(Guid id);
        Task<ProductCategoryResponse> CreateAsync(ProductCategoryRequest request);
        Task<bool> UpdateAsync(Guid id, ProductCategoryRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}

using Contract.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IProductCategoryMappingService
    {
        Task<ProductCategoryMappingResponse> CreateAsync(ProductCategoryMappingRequest request);
        Task<IEnumerable<ProductCategoryMappingResponse>> GetByProductIdAsync(Guid productId);
    }
}

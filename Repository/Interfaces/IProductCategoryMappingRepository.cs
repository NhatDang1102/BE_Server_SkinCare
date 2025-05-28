using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IProductCategoryMappingRepository
    {
        Task<ProductCategoryMapping> CreateAsync(ProductCategoryMapping mapping);
        Task<IEnumerable<ProductCategoryMapping>> GetByProductIdAsync(Guid productId);
    }
}

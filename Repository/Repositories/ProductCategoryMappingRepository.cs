using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class ProductCategoryMappingRepository : IProductCategoryMappingRepository
    {
        private readonly SkinCareAppContext _context;

        public ProductCategoryMappingRepository(SkinCareAppContext context)
        {
            _context = context;
        }

        public async Task<ProductCategoryMapping> CreateAsync(ProductCategoryMapping mapping)
        {
            _context.ProductCategoryMappings.Add(mapping);
            await _context.SaveChangesAsync();
            return mapping;
        }

        public async Task<IEnumerable<ProductCategoryMapping>> GetByProductIdAsync(Guid productId)
        {
            return await _context.ProductCategoryMappings
                .Where(m => m.ProductId == productId)
                .ToListAsync();
        }
    }
}

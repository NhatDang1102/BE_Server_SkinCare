using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Repository.Models;

namespace Repository.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly SkinCareAppContext _context;
        public ProductRepository(SkinCareAppContext context) { _context = context; }

        public async Task<List<SuggestedProduct>> GetAllAsync()
        {
            return await _context.SuggestedProducts.ToListAsync();
        }

        public async Task<SuggestedProduct> GetByIdAsync(Guid id)
        {
            return await _context.SuggestedProducts.FindAsync(id);
        }

        public async Task<List<ProductCategory>> GetCategoriesByProductIdAsync(Guid productId)
        {
            var mappings = await _context.ProductCategoryMappings.Where(x => x.ProductId == productId).ToListAsync();
            var categoryIds = mappings.Select(m => m.CategoryId).ToList();
            return await _context.ProductCategories.Where(c => categoryIds.Contains(c.Id)).ToListAsync();
        }

        public async Task AddAsync(SuggestedProduct product, List<Guid> categoryIds)
        {
            _context.SuggestedProducts.Add(product);
            foreach (var catId in categoryIds)
            {
                _context.ProductCategoryMappings.Add(new ProductCategoryMapping
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    CategoryId = catId,
                    CreatedAt = DateTime.Now
                });
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SuggestedProduct product, List<Guid> categoryIds)
        {
            _context.SuggestedProducts.Update(product);
            var oldMappings = _context.ProductCategoryMappings.Where(x => x.ProductId == product.Id);
            _context.ProductCategoryMappings.RemoveRange(oldMappings);

            foreach (var catId in categoryIds)
            {
                _context.ProductCategoryMappings.Add(new ProductCategoryMapping
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    CategoryId = catId,
                    CreatedAt = DateTime.Now
                });
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(SuggestedProduct product)
        {
            var oldMappings = _context.ProductCategoryMappings.Where(x => x.ProductId == product.Id);
            _context.ProductCategoryMappings.RemoveRange(oldMappings);
            _context.SuggestedProducts.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

}

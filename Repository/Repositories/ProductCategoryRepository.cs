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
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly SkinCareAppContext _context;

        public ProductCategoryRepository(SkinCareAppContext context)
        {
            _context = context;
        }

        public async Task<List<ProductCategory>> GetAllAsync()
            => await _context.ProductCategories.ToListAsync();

        public async Task<ProductCategory> GetByIdAsync(Guid id)
            => await _context.ProductCategories.FindAsync(id);

        public async Task AddAsync(ProductCategory category)
        {
            _context.ProductCategories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductCategory category)
        {
            _context.ProductCategories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductCategory category)
        {
            _context.ProductCategories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}

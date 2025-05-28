using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Repository.Models;

namespace Repository.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly SkinCareAppContext _context;
        public CategoryRepository(SkinCareAppContext context)
        {
            _context = context;
        }

        public async Task<List<ProductCategory>> GetAllAsync()
        {
            return await _context.ProductCategories.ToListAsync();
        }

        public async Task<ProductCategory> GetByIdAsync(Guid id)
        {
            return await _context.ProductCategories.FindAsync(id);
        }

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

        public async Task<bool> AnyProductUsingCategoryAsync(Guid categoryId)
        {
            //check map xem co product k
            return await _context.ProductCategoryMappings.AnyAsync(x => x.CategoryId == categoryId);
        }
    }
}

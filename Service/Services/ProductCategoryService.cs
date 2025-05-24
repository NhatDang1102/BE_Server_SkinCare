using Contract.DTOs;
using Repository.Interfaces;
using Repository.Models;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IProductCategoryRepository _repository;

        public ProductCategoryService(IProductCategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProductCategoryResponse>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync();
            return categories.Select(c => new ProductCategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }

        public async Task<ProductCategoryResponse> GetByIdAsync(Guid id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return null;

            return new ProductCategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }

        public async Task<ProductCategoryResponse> CreateAsync(ProductCategoryRequest request)
        {
            var category = new ProductCategory
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(category);

            return new ProductCategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedAt
            };
        }

        public async Task<bool> UpdateAsync(Guid id, ProductCategoryRequest request)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return false;

            category.Name = request.Name;
            category.Description = request.Description;
            category.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(category);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return false;

            await _repository.DeleteAsync(category);
            return true;
        }
    }
}

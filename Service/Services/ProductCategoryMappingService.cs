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
    public class ProductCategoryMappingService : IProductCategoryMappingService
    {
        private readonly IProductCategoryMappingRepository _repository;

        public ProductCategoryMappingService(IProductCategoryMappingRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductCategoryMappingResponse> CreateAsync(ProductCategoryMappingRequest request)
        {
            var mapping = new ProductCategoryMapping
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                CategoryId = request.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _repository.CreateAsync(mapping);

            return new ProductCategoryMappingResponse
            {
                Id = result.Id,
                ProductId = result.ProductId ?? Guid.Empty,
                CategoryId = result.CategoryId ?? Guid.Empty,
                CreatedAt = result.CreatedAt
            };
        }

        public async Task<IEnumerable<ProductCategoryMappingResponse>> GetByProductIdAsync(Guid productId)
        {
            var mappings = await _repository.GetByProductIdAsync(productId);
            return mappings.Select(m => new ProductCategoryMappingResponse
            {
                Id = m.Id,
                ProductId = m.ProductId ?? Guid.Empty,
                CategoryId = m.CategoryId ?? Guid.Empty,
                CreatedAt = m.CreatedAt
            });
        }
    }
}

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
    public class SuggestedProductService : ISuggestedProductService 
    {
        private readonly ISuggestedProductRepository _repository;
        private readonly IImageUploadService _imageService;
        public SuggestedProductService(
          ISuggestedProductRepository repository,
          IImageUploadService imageService)
        {
            _repository = repository;
            _imageService = imageService;
        }
        public async Task<IEnumerable<SuggestedProductResponse>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();
            return products.Select(p => new SuggestedProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ProductLink = p.ProductLink,
                ImageLink = p.ImageLink,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });
        }
        public async Task<SuggestedProductResponse> GetByIdAsync(Guid id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return null;

            return new SuggestedProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ProductLink = product.ProductLink,
                ImageLink = product.ImageLink,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
        public async Task<SuggestedProductResponse> CreateAsync(SuggestedProductRequest request)
        {
            string imageUrl = null;
            if (request.ImageFile != null)
            {
                imageUrl = await _imageService.UploadImageProductAsync(request.ImageFile);
            }

            var product = new SuggestedProduct
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                ProductLink = request.ProductLink,
                ImageLink = imageUrl,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(product);

            return new SuggestedProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ProductLink = product.ProductLink,
                ImageLink = product.ImageLink,
                CreatedAt = product.CreatedAt
            };
        }
        public async Task<bool> UpdateAsync(Guid id, SuggestedProductRequest request)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return false;

            if (request.ImageFile != null)
            {
                product.ImageLink = await _imageService.UploadImageProductAsync(request.ImageFile);
            }

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.ProductLink = request.ProductLink;
            product.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return false;

            await _repository.DeleteAsync(product);
            return true;
        }
    }

}

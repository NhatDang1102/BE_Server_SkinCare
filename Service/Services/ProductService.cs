using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.DTOs;
using Repository.Interfaces;
using Repository.Models;
using Service.Interfaces;

namespace Service.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly ICategoryRepository _catRepo;

        public ProductService(IProductRepository repo, ICategoryRepository catRepo)
        {
            _repo = repo;
            _catRepo = catRepo;
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            var products = await _repo.GetAllAsync();
            var result = new List<ProductDto>();
            foreach (var p in products)
            {
                var cats = await _repo.GetCategoriesByProductIdAsync(p.Id);
                result.Add(new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ProductLink = p.ProductLink,
                    ImageLink = p.ImageLink,
                    Categories = cats.Select(c => new CategoryDto { Id = c.Id, Name = c.Name, Description = c.Description }).ToList()
                });
            }
            return result;
        }

        public async Task<ProductDto> GetByIdAsync(Guid id)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return null;
            var cats = await _repo.GetCategoriesByProductIdAsync(p.Id);
            return new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ProductLink = p.ProductLink,
                ImageLink = p.ImageLink,
                Categories = cats.Select(c => new CategoryDto { Id = c.Id, Name = c.Name, Description = c.Description }).ToList()
            };
        }

        public async Task<ProductDto> CreateAsync(CreateUpdateProductDto dto)
        {
            //check cate
            if (dto.CategoryIds == null || !dto.CategoryIds.Any())
                throw new Exception("Phai chon category truoc khi add");
            foreach (var catId in dto.CategoryIds)
            {
                var cat = await _catRepo.GetByIdAsync(catId);
                if (cat == null) throw new Exception("Category ko hop le.");
            }
            var entity = new SuggestedProduct
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ProductLink = dto.ProductLink,
                ImageLink = dto.ImageLink,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            await _repo.AddAsync(entity, dto.CategoryIds);
            var cats = await _repo.GetCategoriesByProductIdAsync(entity.Id);
            return new ProductDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                ProductLink = entity.ProductLink,
                ImageLink = entity.ImageLink,
                Categories = cats.Select(c => new CategoryDto { Id = c.Id, Name = c.Name, Description = c.Description }).ToList()
            };
        }

        public async Task<ProductDto> UpdateAsync(Guid id, CreateUpdateProductDto dto)
        {

            //check cate
            if (dto.CategoryIds == null || !dto.CategoryIds.Any())
                throw new Exception("Phai chon category truoc khi add!");
            foreach (var catId in dto.CategoryIds)
            {
                var cat = await _catRepo.GetByIdAsync(catId);
                if (cat == null) throw new Exception("Category ko hop le.");
            }


            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Price = dto.Price;
            entity.ProductLink = dto.ProductLink;
            entity.ImageLink = dto.ImageLink;
            entity.UpdatedAt = DateTime.Now;
            await _repo.UpdateAsync(entity, dto.CategoryIds);
            var cats = await _repo.GetCategoriesByProductIdAsync(entity.Id);
            return new ProductDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                ProductLink = entity.ProductLink,
                ImageLink = entity.ImageLink,
                Categories = cats.Select(c => new CategoryDto { Id = c.Id, Name = c.Name, Description = c.Description }).ToList()
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;
            await _repo.DeleteAsync(entity);
            return true;
        }
    }

}

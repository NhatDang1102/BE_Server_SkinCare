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
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        public CategoryService(ICategoryRepository repo) { _repo = repo; }

        public async Task<List<CategoryDto>> GetAllAsync()
            => (await _repo.GetAllAsync()).Select(x => new CategoryDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToList();

        public async Task<CategoryDto> GetByIdAsync(Guid id)
        {
            var x = await _repo.GetByIdAsync(id);
            return x == null ? null : new CategoryDto { Id = x.Id, Name = x.Name, Description = x.Description };
        }

        public async Task<CategoryDto> CreateAsync(CreateUpdateCategoryDto dto)
        {
            //check dupe cate
            var exists = (await _repo.GetAllAsync()).Any(x => x.Name.Trim().ToLower() == dto.Name.Trim().ToLower());
            if (exists)
                throw new Exception("Category da ton tai.");
            var entity = new ProductCategory { Id = Guid.NewGuid(), Name = dto.Name, Description = dto.Description, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
            await _repo.AddAsync(entity);
            return new CategoryDto { Id = entity.Id, Name = entity.Name, Description = entity.Description };
        }

        public async Task<CategoryDto> UpdateAsync(Guid id, CreateUpdateCategoryDto dto)
        {
            //check dupe cate
            var exists = (await _repo.GetAllAsync()).Any(x => x.Name.Trim().ToLower() == dto.Name.Trim().ToLower() && x.Id != id);
            if (exists)
                throw new Exception("Category da ton tai.");
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;
            entity.Name = dto.Name; entity.Description = dto.Description; entity.UpdatedAt = DateTime.Now;
            await _repo.UpdateAsync(entity);
            return new CategoryDto { Id = entity.Id, Name = entity.Name, Description = entity.Description };
        }


        public async Task<bool> DeleteAsync(Guid id)
        {
            //check xem cate co product nao k
            var anyProduct = await _repo.AnyProductUsingCategoryAsync(id);
            if (anyProduct)
                throw new Exception("Dang co product trong cate nay. Xoa het product trc khi xoa cate");

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;
            await _repo.DeleteAsync(entity);
            return true;
        }
    }
}

using Contract.DTOs;
using Repository.Interfaces;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class BlogService : IBlogService
{
    private readonly IBlogRepository _repo;
    private readonly IProductRepository _productRepo;

    public BlogService(IBlogRepository repo, IProductRepository productRepo)
    {
        _repo = repo;
        _productRepo = productRepo;
    }

    public async Task<List<BlogDto>> GetAllAsync()
    {
        var blogs = await _repo.GetAllAsync();
        var result = new List<BlogDto>();
        foreach (var b in blogs)
        {
            ProductDto productDto = null;
            if (b.ProductId != null && b.Product != null)
            {
                productDto = new ProductDto
                {
                    Id = b.Product.Id,
                    Name = b.Product.Name,
                    Description = b.Product.Description,
                    Price = b.Product.Price,
                    ProductLink = b.Product.ProductLink,
                    ImageLink = b.Product.ImageLink
                };
            }
            result.Add(new BlogDto
            {
                Id = b.Id,
                Title = b.Title,
                Content = b.Content,
                ProductId = b.ProductId,
                ExternalProductLink = b.ExternalProductLink,
                Product = productDto,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            });
        }
        return result;
    }

    public async Task<BlogDto> GetByIdAsync(Guid id)
    {
        var b = await _repo.GetByIdAsync(id);
        if (b == null) return null;
        ProductDto productDto = null;
        if (b.ProductId != null && b.Product != null)
        {
            productDto = new ProductDto
            {
                Id = b.Product.Id,
                Name = b.Product.Name,
                Description = b.Product.Description,
                Price = b.Product.Price,
                ProductLink = b.Product.ProductLink,
                ImageLink = b.Product.ImageLink
            };
        }
        return new BlogDto
        {
            Id = b.Id,
            Title = b.Title,
            Content = b.Content,
            ProductId = b.ProductId,
            ExternalProductLink = b.ExternalProductLink,
            Product = productDto,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt
        };
    }

    public async Task<BlogDto> CreateAsync(CreateUpdateBlogDto dto)
    {
        if (dto.ProductId != null)
        {
            var product = await _productRepo.GetByIdAsync(dto.ProductId.Value);
            if (product == null) throw new Exception("product id sai invalid");
        }
        if (dto.ProductId == null && string.IsNullOrWhiteSpace(dto.ExternalProductLink))
            throw new Exception("phai co link ngoai` neu product ko co link/ko co product");

        var entity = new Blog
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Content = dto.Content,
            ProductId = dto.ProductId,
            ExternalProductLink = dto.ExternalProductLink,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        await _repo.AddAsync(entity);

        ProductDto productDto = null;
        if (entity.ProductId != null)
        {
            var product = await _productRepo.GetByIdAsync(entity.ProductId.Value);
            if (product != null)
            {
                productDto = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    ProductLink = product.ProductLink,
                    ImageLink = product.ImageLink
                };
            }
        }
        return new BlogDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Content = entity.Content,
            ProductId = entity.ProductId,
            ExternalProductLink = entity.ExternalProductLink,
            Product = productDto,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public async Task<BlogDto> UpdateAsync(Guid id, CreateUpdateBlogDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return null;

        if (dto.ProductId != null)
        {
            var product = await _productRepo.GetByIdAsync(dto.ProductId.Value);
            if (product == null) throw new Exception("product id sai invalid");
        }
        if (dto.ProductId == null && string.IsNullOrWhiteSpace(dto.ExternalProductLink))
            throw new Exception("phai co link ngoai` neu product ko co link/ko co product");

        entity.Title = dto.Title;
        entity.Content = dto.Content;
        entity.ProductId = dto.ProductId;
        entity.ExternalProductLink = dto.ExternalProductLink;
        entity.UpdatedAt = DateTime.Now;
        await _repo.UpdateAsync(entity);

        ProductDto productDto = null;
        if (entity.ProductId != null)
        {
            var product = await _productRepo.GetByIdAsync(entity.ProductId.Value);
            if (product != null)
            {
                productDto = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    ProductLink = product.ProductLink,
                    ImageLink = product.ImageLink
                };
            }
        }
        return new BlogDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Content = entity.Content,
            ProductId = entity.ProductId,
            ExternalProductLink = entity.ExternalProductLink,
            Product = productDto,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
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

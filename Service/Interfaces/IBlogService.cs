using Contract.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IBlogService
{
    Task<List<BlogDto>> GetAllAsync();
    Task<BlogDto> GetByIdAsync(Guid id);
    Task<BlogDto> CreateAsync(CreateUpdateBlogDto dto);
    Task<BlogDto> UpdateAsync(Guid id, CreateUpdateBlogDto dto);
    Task<bool> DeleteAsync(Guid id);
}

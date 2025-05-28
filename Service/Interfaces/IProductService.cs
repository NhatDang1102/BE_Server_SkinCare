using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.DTOs;

namespace Service.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllAsync();
        Task<ProductDto> GetByIdAsync(Guid id);
        Task<ProductDto> CreateAsync(CreateUpdateProductDto dto);
        Task<ProductDto> UpdateAsync(Guid id, CreateUpdateProductDto dto);
        Task<bool> DeleteAsync(Guid id);
    }

}

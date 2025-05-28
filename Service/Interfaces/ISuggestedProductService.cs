using Contract.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface ISuggestedProductService
    {
        Task<IEnumerable<SuggestedProductResponse>> GetAllAsync();
        Task<SuggestedProductResponse> GetByIdAsync(Guid id);
        Task<SuggestedProductResponse> CreateAsync(SuggestedProductRequest request);
        Task<bool> UpdateAsync(Guid id, SuggestedProductRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}

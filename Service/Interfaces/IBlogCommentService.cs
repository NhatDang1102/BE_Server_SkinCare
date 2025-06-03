using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IBlogCommentService
    {
        Task<List<BlogCommentDto>> GetByBlogIdAsync(Guid blogId);
        Task<BlogCommentDto> CreateAsync(Guid userId, CreateUpdateBlogCommentDto dto);
        Task<BlogCommentDto> UpdateAsync(Guid userId, Guid commentId, string commentText, string userRole);
        Task<bool> DeleteAsync(Guid userId, Guid commentId, string userRole);
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Models;

namespace Repository.Interfaces
{
    public interface IBlogCommentRepository
    {
        Task<List<BlogComment>> GetByBlogIdAsync(Guid blogId);
        Task<BlogComment> GetByIdAsync(Guid id);
        Task AddAsync(BlogComment comment);
        Task UpdateAsync(BlogComment comment);
        Task DeleteAsync(BlogComment comment);
    }

}

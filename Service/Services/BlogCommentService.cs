using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Interfaces;
using Repository.Models;
using Service.Interfaces;

namespace Service.Services
{
    public class BlogCommentService : IBlogCommentService
    {
        private readonly IBlogCommentRepository _repo;
        private readonly IUserRepository _userRepo;

        public BlogCommentService(IBlogCommentRepository repo, IUserRepository userRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
        }

        public async Task<List<BlogCommentDto>> GetByBlogIdAsync(Guid blogId)
        {
            var comments = await _repo.GetByBlogIdAsync(blogId);
            return comments.Select(c => new BlogCommentDto
            {
                Id = c.Id,
                BlogId = c.BlogId ?? Guid.Empty,
                UserId = c.UserId ?? Guid.Empty,
                CommentText = c.CommentText,
                CreatedAt = c.CreatedAt,
                User_Id = c.User?.Id,
                User_Name = c.User?.Name,
                User_Avatar = c.User?.ProfilePicture
            }).ToList();
        }

        public async Task<BlogCommentDto> CreateAsync(Guid userId, CreateUpdateBlogCommentDto dto)
        {
            var entity = new BlogComment
            {
                Id = Guid.NewGuid(),
                BlogId = dto.BlogId,
                UserId = userId,
                CommentText = dto.CommentText,
                CreatedAt = DateTime.Now
            };
            await _repo.AddAsync(entity);

            var user = await _userRepo.GetByIdAsync(userId);
            return new BlogCommentDto
            {
                Id = entity.Id,
                BlogId = entity.BlogId ?? Guid.Empty,
                UserId = entity.UserId ?? Guid.Empty,
                CommentText = entity.CommentText,
                CreatedAt = entity.CreatedAt,
                User_Id = user?.Id,
                User_Name = user?.Name,
                User_Avatar = user?.ProfilePicture
            };
        }

        public async Task<BlogCommentDto> UpdateAsync(Guid userId, Guid commentId, string commentText, string userRole)
        {
            var entity = await _repo.GetByIdAsync(commentId);
            if (entity == null) return null;
            if (entity.UserId != userId && userRole != "Admin" && userRole != "Manager")
                throw new Exception("no permission");

            entity.CommentText = commentText;
            await _repo.UpdateAsync(entity);

            var user = await _userRepo.GetByIdAsync(entity.UserId ?? Guid.Empty);
            return new BlogCommentDto
            {
                Id = entity.Id,
                BlogId = entity.BlogId ?? Guid.Empty,
                UserId = entity.UserId ?? Guid.Empty,
                CommentText = entity.CommentText,
                CreatedAt = entity.CreatedAt,
                User_Id = user?.Id,
                User_Name = user?.Name,
                User_Avatar = user?.ProfilePicture
            };
        }

        public async Task<bool> DeleteAsync(Guid userId, Guid commentId, string userRole)
        {
            var entity = await _repo.GetByIdAsync(commentId);
            if (entity == null) return false;
            if (entity.UserId != userId && userRole != "Admin" && userRole != "Manager")
                throw new Exception("no permission");
            await _repo.DeleteAsync(entity);
            return true;
        }
    }

}

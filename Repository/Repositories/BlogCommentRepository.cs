using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Repository.Models;

public class BlogCommentRepository : IBlogCommentRepository
{
    private readonly SkinCareAppContext _context;
    public BlogCommentRepository(SkinCareAppContext context) => _context = context;

    public async Task<List<BlogComment>> GetByBlogIdAsync(Guid blogId)
    {
        return await _context.BlogComments
            .Include(c => c.User)
            .Where(c => c.BlogId == blogId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<BlogComment> GetByIdAsync(Guid id)
    {
        return await _context.BlogComments
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(BlogComment comment)
    {
        _context.BlogComments.Add(comment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(BlogComment comment)
    {
        _context.BlogComments.Update(comment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(BlogComment comment)
    {
        _context.BlogComments.Remove(comment);
        await _context.SaveChangesAsync();
    }
}
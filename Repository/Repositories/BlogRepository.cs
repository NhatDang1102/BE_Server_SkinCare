using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Repository.Models;

namespace Repository.Repositories
{
    public class BlogRepository : IBlogRepository
    {
        private readonly SkinCareAppContext _context;
        public BlogRepository(SkinCareAppContext context) => _context = context;

        public async Task<List<Blog>> GetAllAsync()
        {
            return await _context.Blogs
                .Include(b => b.Product) 
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Blog> GetByIdAsync(Guid id)
        {
            return await _context.Blogs
                .Include(b => b.Product)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddAsync(Blog blog)
        {
            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Blog blog)
        {
            _context.Blogs.Update(blog);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Blog blog)
        {
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
        }
    }

}

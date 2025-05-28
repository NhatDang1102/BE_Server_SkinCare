using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class SuggestedProductRepository : ISuggestedProductRepository
    {
        private readonly SkinCareAppContext _context;

        public SuggestedProductRepository(SkinCareAppContext context)
        {
            _context = context;
        }

        public async Task<List<SuggestedProduct>> GetAllAsync()
            => await _context.SuggestedProducts.ToListAsync();

        public async Task<SuggestedProduct> GetByIdAsync(Guid id)
            => await _context.SuggestedProducts.FindAsync(id);

        public async Task AddAsync(SuggestedProduct product)
        {
            _context.SuggestedProducts.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SuggestedProduct product)
        {
            _context.SuggestedProducts.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(SuggestedProduct product)
        {
            _context.SuggestedProducts.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}

using Repository.DTOs;
using Repository.Models;

namespace Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetByEmailAsync(string email);
        Task UpdateAsync(User user);
        Task SaveChangesAsync();
    }
}

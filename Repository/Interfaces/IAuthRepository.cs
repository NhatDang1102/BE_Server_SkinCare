using System.Threading.Tasks;
using Repository.Models;

namespace Repository.Interfaces
{
    public interface IAuthRepository
    {
        Task<bool> EmailExistsAsync(string email);
        Task AddTempUserAsync(TempUser tempUser);
        Task<TempUser> GetTempUserByEmailAsync(string email);
        Task DeleteTempUserAsync(string email);
        Task UpdateAsync(User user);

        Task<User> GetUserByEmailAsync(string email);
        Task CreateUserAsync(User user);
    }
}

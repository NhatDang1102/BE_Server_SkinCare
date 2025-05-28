using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.DTOs;

namespace Service.Interfaces
{
    public interface IAdminService
    {
        Task<List<UserSimpleDto>> GetAllUsersAsync();
        Task<bool> UpdateUserStatusAsync(UpdateUserStatusDto dto);
    }
}

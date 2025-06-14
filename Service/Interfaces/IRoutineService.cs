using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.DTOs;
using Microsoft.AspNetCore.Http;

namespace Service.Interfaces
{
    public interface IRoutineService
    {
        Task<RoutineResultDto> AnalyzeAndSaveRoutineAsync(Guid userId, IFormFile image);
        Task<RoutineResultFullDto> GetRoutineByUserIdAsync(Guid userId);

    }
}

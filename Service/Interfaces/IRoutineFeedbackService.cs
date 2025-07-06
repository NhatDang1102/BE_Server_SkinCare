using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.DTOs;

namespace Service.Interfaces
{
    public interface IRoutineFeedbackService
    {
        Task SubmitFeedbackAsync(Guid userId, RoutineFeedbackCreateDto dto);
        Task<List<RoutineFeedbackAdminDto>> GetAllFeedbacksAsync();

    }
}

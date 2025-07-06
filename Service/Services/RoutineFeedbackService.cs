using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.DTOs;
using Repository.Models;
using Service.Interfaces;

namespace Service.Services
{
    public class RoutineFeedbackService : IRoutineFeedbackService
    {
        private readonly IRoutineRepository _repo;
        private readonly IImageUploadService _imageService;
        public RoutineFeedbackService(IRoutineRepository repo, IImageUploadService imageService)
        {
            _repo = repo;
            _imageService = imageService;
        }

        public async Task SubmitFeedbackAsync(Guid userId, RoutineFeedbackCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Message))
                throw new Exception("Nội dung feedback không được để trống.");

            var routine = await _repo.GetByUserIdAsync(userId);
            if (routine == null)
                throw new Exception("Bạn chưa có routine để gửi feedback!");

            string imgUrl = null;
            if (dto.Image != null && dto.Image.Length > 0)
                imgUrl = await _imageService.UploadProductImageAsync(dto.Image);

            var feedback = new RoutineFeedback
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoutineId = routine.Id,
                Message = dto.Message,
                ImageUrl = imgUrl,
                CreatedAt = DateTime.UtcNow
            };
            await _repo.AddAsync(feedback);
        }


        public async Task<List<RoutineFeedbackAdminDto>> GetAllFeedbacksAsync()
        {
            return await _repo.GetAllFeedbacksAsync();
        }
    }

}

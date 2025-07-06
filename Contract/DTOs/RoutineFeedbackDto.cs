using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Contract.DTOs
{
    public class RoutineFeedbackCreateDto
    {
        public string Message { get; set; }
        public IFormFile? Image { get; set; }
    }


    public class RoutineFeedbackAdminDto
    {
        public Guid Id { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public Guid RoutineId { get; set; }
        public string Message { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }


}

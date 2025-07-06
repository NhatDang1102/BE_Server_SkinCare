using System;

namespace Repository.Models
{
    public partial class RoutineFeedback
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RoutineId { get; set; }
        public string Message { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual User User { get; set; }
        public virtual DailyRoutine Routine { get; set; }
    }
}

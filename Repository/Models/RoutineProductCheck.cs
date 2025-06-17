using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    public partial class RoutineProductCheck
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RoutineId { get; set; }
        public Guid ProductId { get; set; }
        public string Session { get; set; } // "morning", "noon", "night"
        public DateTime UsageDate { get; set; }
        public bool IsChecked { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual User User { get; set; }
        public virtual DailyRoutine Routine { get; set; }
        public virtual SuggestedProduct Product { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.DTOs
{
    public class PaymentLogDto
    {
        public string UserEmail { get; set; }
        public string PackageName { get; set; }
        public decimal? PaymentAmount { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? PaymentDate { get; set; }
    }

}

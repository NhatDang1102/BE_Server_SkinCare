using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.DTOs
{
    public class LoginHistoryDto
    {
        public string Ip { get; set; }
        public string Device { get; set; }
        public DateTime LoginAt { get; set; }
    }

}
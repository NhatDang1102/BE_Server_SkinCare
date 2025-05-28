using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.DTOs
{
    public class UpdateUserStatusDto
    {
        public Guid UserId { get; set; }
        public bool IsActive { get; set; }
    }
}

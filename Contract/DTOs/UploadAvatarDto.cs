using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Contract.DTOs
{
    public class UploadAvatarDto
    {
        public IFormFile ImageFile { get; set; }
    }

}

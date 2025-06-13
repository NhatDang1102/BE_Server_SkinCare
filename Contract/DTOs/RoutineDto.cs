using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Contract.DTOs
{
    public class RoutineRequestDto
    {
        public IFormFile Image { get; set; }
    }

    public class RoutineResultDto
    {
        public List<ProductDto> Morning { get; set; }
        public List<ProductDto> Noon { get; set; }
        public List<ProductDto> Night { get; set; }
        public string Advice { get; set; }
    }

    public class RoutineResultFullDto
    {
        public List<ProductDto> Morning { get; set; }
        public List<ProductDto> Noon { get; set; }
        public List<ProductDto> Night { get; set; }
        public string Advice { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

}

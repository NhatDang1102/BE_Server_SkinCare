using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public string ProductLink { get; set; }
        public string ImageLink { get; set; }
        public List<CategoryDto> Categories { get; set; }
    }

    public class CreateUpdateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public string ProductLink { get; set; }
        public string ImageLink { get; set; }
        public List<Guid> CategoryIds { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.DTOs
{
    public class ProductCategoryMappingRequest
    {
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }
    }

    public class ProductCategoryMappingResponse
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

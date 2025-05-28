using Contract.DTOs;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Main.Controllers
{
    [ApiController]
    [Route("SkinCare/product-category-mappings")]
    public class ProductCategoryMappingsController : ControllerBase
    {
        private readonly IProductCategoryMappingService _service;

        public ProductCategoryMappingsController(IProductCategoryMappingService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCategoryMappingRequest request)
        {
            var result = await _service.CreateAsync(request);
            return Ok(result);
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(Guid productId)
        {
            var result = await _service.GetByProductIdAsync(productId);
            return Ok(result);
        }
    }
}

using Contract.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Main.Controllers
{
    [ApiController]
    [Route("SkinCare/Product")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly IImageUploadService _imageUploadService;

        public ProductController(IProductService service, IImageUploadService imageUploadService)
        {
            _service = service;
            _imageUploadService = imageUploadService;
        }

        // Tạo sản phẩm (upload ảnh luôn)
        [HttpPost("create")]
        [Authorize(Roles = "Manager,Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateAsync([FromForm] CreateUpdateProductWithFileDto dto)
        {
            try
            {
                string imageLink = null;
                if (dto.Image != null && dto.Image.Length > 0)
                {
                    imageLink = await _imageUploadService.UploadProductImageAsync(dto.Image);
                }
                var createDto = new CreateUpdateProductDto
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    ProductLink = dto.ProductLink,
                    ImageLink = imageLink,
                    CategoryIds = dto.CategoryIds
                };
                var result = await _service.CreateAsync(createDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Sửa sản phẩm (có thể upload lại ảnh mới)
        [HttpPut("{id}/update")]
        [Authorize(Roles = "Manager,Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromForm] CreateUpdateProductWithFileDto dto)
        {
            try
            {
                string imageLink = null;
                if (dto.Image != null && dto.Image.Length > 0)
                {
                    imageLink = await _imageUploadService.UploadProductImageAsync(dto.Image);
                }

                var updateDto = new CreateUpdateProductDto
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    ProductLink = dto.ProductLink,
                    ImageLink = imageLink, // hoặc giữ link cũ nếu muốn
                    CategoryIds = dto.CategoryIds
                };
                var result = await _service.UpdateAsync(id, updateDto);
                return result == null ? NotFound() : Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Lấy tất cả
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _service.GetAllAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Lấy theo id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var p = await _service.GetByIdAsync(id);
                return p == null ? NotFound() : Ok(p);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Xóa
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var ok = await _service.DeleteAsync(id);
                return ok ? Ok(new { message = "Xóa thành công" }) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

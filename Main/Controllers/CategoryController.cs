using Contract.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Services;

namespace Main.Controllers
{
    [ApiController]
    [Route("SkinCare/Category")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;
        public CategoryController(ICategoryService service) { _service = service; }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var cats = await _service.GetAllAsync();
                return Ok(cats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var cat = await _service.GetByIdAsync(id);
                return cat == null ? NotFound() : Ok(cat);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
 
        }

        [HttpPost("create")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Create(CreateUpdateCategoryDto dto)
        {
            try
            {
                var cat = await _service.CreateAsync(dto);
                return Ok(cat);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,Admin")]

        public async Task<IActionResult> Update(Guid id, CreateUpdateCategoryDto dto)
        {
          
            try
            {
                var cat = await _service.UpdateAsync(id, dto);
                return cat == null ? NotFound() : Ok(cat);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager,Admin")]

        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var ok = await _service.DeleteAsync(id);
                return ok ? Ok(new { message = "Da xoa" }) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
    
        }
    }

}

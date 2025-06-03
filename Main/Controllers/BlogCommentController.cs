using Contract.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Main.Controllers
{
    [ApiController]
    [Route("SkinCare/Blog/Comment")]
    public class BlogCommentController : ControllerBase
    {
        private readonly IBlogCommentService _service;

        public BlogCommentController(IBlogCommentService service)
        {
            _service = service;
        }

        [HttpGet("blog/{blogId}")]
        public async Task<IActionResult> GetByBlogId(Guid blogId)
        {
            try
            {
                var list = await _service.GetByBlogIdAsync(blogId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateUpdateBlogCommentDto dto)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var result = await _service.CreateAsync(userId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateUpdateBlogCommentDto dto)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "User";
                var result = await _service.UpdateAsync(userId, id, dto.CommentText, role);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "User";
                var ok = await _service.DeleteAsync(userId, id, role);
                return ok ? Ok(new { message = "Xóa thành công" }) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

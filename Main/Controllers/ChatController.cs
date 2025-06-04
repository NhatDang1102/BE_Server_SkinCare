using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System.Security.Claims;

[ApiController]
[Route("SkinCare/AIChat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _service;
    public ChatController(IChatService service)
    {
        _service = service;
    }

    [HttpPost("ask")]
    [Authorize]
    public async Task<IActionResult> Ask([FromBody] string prompt)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var response = await _service.ChatAsync(userId, prompt);
        return Ok(new { response });
    }

    [HttpGet("history")]
    [Authorize]
    public async Task<IActionResult> History([FromQuery] int take = 20)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var history = await _service.GetHistoryAsync(userId, take);
        return Ok(history);
    }
}

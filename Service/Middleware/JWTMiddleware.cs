using Microsoft.AspNetCore.Http;
using Service.Interfaces;
using System.Linq;
using System.Threading.Tasks;

public class JwtBlacklistMiddleware
{
    private readonly RequestDelegate _next;

    public JwtBlacklistMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IRedisService redisService)
    {
        string token = null;

        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            token = authHeader.Substring("Bearer ".Length).Trim();
        }

        if (string.IsNullOrEmpty(token) && context.Request.Cookies.ContainsKey("access_token"))
        {
            token = context.Request.Cookies["access_token"];
        }

        if (!string.IsNullOrEmpty(token))
        {
            var blacklistKey = $"blacklist:{token}";
            var isBlacklisted = await redisService.GetStringAsync(blacklistKey);

            if (!string.IsNullOrEmpty(isBlacklisted))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var json = "{\"message\":\"token này dính blacklist\"}";
                await context.Response.WriteAsync(json);
                return;
            }
        }
        await _next(context);
    }


}

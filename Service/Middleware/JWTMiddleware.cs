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
        //middleware check cookie, xong check key tren redis
        
        if (context.Request.Cookies.ContainsKey("access_token"))
        {
            token = context.Request.Cookies["access_token"];
        }

        if (!string.IsNullOrEmpty(token))
        {
            var blacklistKey = $"blacklist:{token}";
            var isBlacklisted = await redisService.GetStringAsync(blacklistKey);
            //return plain text message
            // if (!string.IsNullOrEmpty(isBlacklisted))
            // {
            //   context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            // await context.Response.WriteAsync("token này dính blacklist");
            //return;
            //}
            //return json 
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

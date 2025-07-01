using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Repository.Interfaces;
using Repository.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

public class UserVipRequiredAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userIdStr = context.HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            context.Result = new JsonResult(new { message = "Không xác định được user" }) { StatusCode = 401 };
            return;
        }

        var userVipRepo = context.HttpContext.RequestServices.GetService(typeof(IUserVipRepository)) as IUserVipRepository;
        if (userVipRepo == null)
        {
            context.Result = new JsonResult(new { message = "Không xác định repository" }) { StatusCode = 500 };
            return;
        }

        var isVip = await userVipRepo.IsUserVipAsync(userId);
        if (!isVip)
        {
            context.Result = new JsonResult(new { message = "Bạn cần mua gói VIP để dùng chức năng này." }) { StatusCode = 403 };
            return;
        }

        await next();
    }
}

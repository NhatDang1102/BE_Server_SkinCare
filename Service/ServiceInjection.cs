using Microsoft.Extensions.DependencyInjection;
using Repository.Interfaces;
using Repository.Repositories;
using Service.Interfaces;
using Service.Services;

namespace Service
{
    public static class ServiceInjection
    {
        public static IServiceCollection AddServiceServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IImageUploadService, ImageUploadService>();
            services.AddScoped<IPasswordResetService, PasswordResetService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IOpenAiService, OpenAiService>();
            services.AddScoped<IBlogCommentService, BlogCommentService>();
            services.AddScoped<IOpenAiVisionService, OpenAiVisionService>();
            services.AddScoped<IRoutineService, RoutineService>();
            services.AddScoped<IVipPaymentService, VipPaymentService>();
            services.AddScoped<IRoutineFeedbackService, RoutineFeedbackService>();
            services.AddSingleton<IRedisService, RedisService>();

            return services;
        }
    }
}

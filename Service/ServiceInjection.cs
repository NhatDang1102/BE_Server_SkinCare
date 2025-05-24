using Microsoft.Extensions.DependencyInjection;
using Service.Interfaces;
using Service.Services;

namespace Service
{
    public static class ServiceInjection
    {
        public static IServiceCollection AddServiceServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProductCategoryService, ProductCategoryService>();

            return services;
        }
    }
}

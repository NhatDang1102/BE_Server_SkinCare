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
            services.AddScoped<IImageUploadService, CloudinaryImageUploadService>();
            services.AddScoped<ISuggestedProductService, SuggestedProductService>();
            services.AddScoped<IProductCategoryMappingService, ProductCategoryMappingService>();
            return services;
        }
    }
}

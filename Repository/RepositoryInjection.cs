using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Repository.Interfaces;
using Repository.Repositories;

namespace Repository
{
    public static class RepositoryInjection
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthRepository,AuthRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBlogRepository, BlogRepository>();
            services.AddScoped<IBlogCommentRepository, BlogCommentRepository>();
            services.AddScoped<IRoutineRepository, RoutineRepository>();

            return services;
        }
    }
}

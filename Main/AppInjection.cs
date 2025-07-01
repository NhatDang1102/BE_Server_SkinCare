using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.Helpers;
using Repository.Models;
using Repository.DTOs;
using Contract.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Net.payOS;

namespace MainApp
{
    public static class AppInjection
    {
        public static IServiceCollection AddMainAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            services.Configure<JwtSettings>(configuration.GetSection("Jwt")); 
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            services.Configure<RedisSettings>(configuration.GetSection("Redis"));

            PayOS payOS = new PayOS(
    configuration["Environment:PAYOS_CLIENT_ID"] ?? throw new Exception("Cannot find PAYOS_CLIENT_ID"),
    configuration["Environment:PAYOS_API_KEY"] ?? throw new Exception("Cannot find PAYOS_API_KEY"),
    configuration["Environment:PAYOS_CHECKSUM_KEY"] ?? throw new Exception("Cannot find PAYOS_CHECKSUM_KEY")
); 
            services.AddSingleton(payOS);
            services.AddSingleton<MailSender>();
            services.AddDbContext<SkinCareAppContext>();
            services.AddHttpContextAccessor();

            services.AddAuthentication("Bearer")
          .AddJwtBearer(options =>
          {
              options.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuer = false,
                  ValidateAudience = false,
                  ValidateLifetime = true,
                  ValidateIssuerSigningKey = true,
                  IssuerSigningKey = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
              };

              // Thêm code này để lấy token từ Cookie nếu không có trong header
              options.Events = new JwtBearerEvents
              {
                  OnMessageReceived = context =>
                  {
                      //lay token tu cookie
                      if (string.IsNullOrEmpty(context.Token))
                      {
                          var cookieToken = context.Request.Cookies["access_token"];
                          if (!string.IsNullOrEmpty(cookieToken))
                              context.Token = cookieToken;
                      }
                      return Task.CompletedTask;
                  }
              };
          });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllFE", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:5173", "https://exeskincare.onrender.com", "https://192.168.1.9:5173", "https://localhost:5173", "https://exe201-skincare-fe-new.onrender.com", "https://skinsenseteam.vercel.app")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });


            return services;
        }
    }
}

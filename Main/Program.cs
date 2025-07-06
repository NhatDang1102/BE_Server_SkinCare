using System.Text;
using Contract.Helpers;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using MainApp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Repository.Interfaces;
using Repository.Models;
using Repository.Repositories;
using Service;
using Service.Helpers;
using Service.Interfaces;
using Service.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Secret.json", optional: true, reloadOnChange: true);
// Add services to the container.
//(Da add ben appinjection)
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
    return ConnectionMultiplexer.Connect(settings.ConnectionString);
});
builder.Services.AddMainAppServices(builder.Configuration);
builder.Services.AddRepositoryServices();
builder.Services.AddServiceServices();





var app = builder.Build();

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("skincare-f2d06-firebase-adminsdk-fbsvc-85448f1f3b.json")
});

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAllFE");


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseMiddleware<JwtBlacklistMiddleware>();
app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();

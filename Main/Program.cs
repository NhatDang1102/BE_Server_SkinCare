using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using MainApp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Repository.Interfaces;
using Repository.Models;
using Repository.Repositories;
using Service;
using Service.Helpers;
using Service.Interfaces;
using Service.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Secret.json", optional: true, reloadOnChange: true);
// Add services to the container.
//(Da add ben appinjection)
builder.Services.AddControllers();
// Learn more about  configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseCors("localhostFE");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

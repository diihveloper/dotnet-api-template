using DiihTemplate.Application;
using DiihTemplate.Core;
using DiihTemplate.Core.Middlewares;
using DiihTemplate.Data;
using DiihTemplate.Domain;
using DiihTemplate.Infra;
using Microsoft.AspNetCore.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddIniFile("appsettings.ini", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();

// DiihTemplate
builder.Services.AddDiihTemplateDbContext(builder.Configuration.GetValue<string>("Database"),
    builder.Configuration.GetConnectionString("Default"));
builder.Services.AddDiihTemplateApplicationServices();
builder.Services.AddDiihTemplateCore();
builder.Services.AddDiihTemplateInfra(builder.Configuration);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Host.UseSerilog();

// Auth
builder.Services.AddAuthentication();
builder.Services.AddAuthorization(options =>
{
    // add policies here
});

builder.Services
    .AddIdentityApiEndpoints<AppUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DiihTemplateDbContext>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<HandleExceptionMiddleware>();
app.UseMiddleware<UnitOfWorkMiddleware>();
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<AppUser>().AddEndpointFilter(async (context, next) =>
{
    if (context.HttpContext.Request.Path.StartsWithSegments("/register", StringComparison.OrdinalIgnoreCase))
    {
        return Results.NotFound();
    }

    return await next(context);
});

app.UseAuthorization();

app.MapControllers();

app.Run();


public partial class Program
{
}
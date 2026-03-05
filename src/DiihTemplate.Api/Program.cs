using System.Threading.RateLimiting;
using DiihTemplate.Application;
using DiihTemplate.Core;
using DiihTemplate.Core.Middlewares;
using DiihTemplate.Data;
using DiihTemplate.Domain;
using DiihTemplate.Infra;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
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
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});
builder.Services.AddOpenApi();
builder.Host.UseSerilog();

// CORS
var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
                  ?? [builder.Configuration.GetValue<string>("Cors:Origin") ?? "http://localhost:4200"];
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<DiihTemplateDbContext>();

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });
});

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

app.UseCors();
app.UseRateLimiter();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapIdentityApi<AppUser>().AddEndpointFilter(async (context, next) =>
{
    if (context.HttpContext.Request.Path.StartsWithSegments("/register", StringComparison.OrdinalIgnoreCase))
    {
        return Results.NotFound();
    }

    return await next(context);
});

app.MapControllers();

app.Run();


public partial class Program
{
}

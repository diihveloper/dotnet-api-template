using DiihTemplate.Application;
using DiihTemplate.Core.Middlewares;
using DiihTemplate.Data;
using DiihTemplate.Infra;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddIniFile("appsettings.ini", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddHttpContextAccessor();
builder.Services.AddDiihTemplateDbContext(builder.Configuration.GetValue<string>("Database"),
    builder.Configuration.GetConnectionString("Default"));
builder.Services.AddDiihTemplateApplicationServices();
builder.Services.AddDiihTemplateInfra(builder.Configuration);
builder.Services.AddDistributedMemoryCache();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<HandleExceptionMiddleware>();
app.UseMiddleware<UnitOfWorkMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public partial class Program
{
}
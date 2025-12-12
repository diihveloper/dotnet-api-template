using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DiihTemplate.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddDiihTemplateApplicationServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        // services.AddScoped<IAppService, AppService>();
        
        return services;
    }
}

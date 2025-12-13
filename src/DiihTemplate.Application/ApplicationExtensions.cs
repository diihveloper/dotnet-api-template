using DiihTemplate.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DiihTemplate.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddDiihTemplateApplicationServices(this IServiceCollection services)
    {
        services.AddDiihTemplateCoreEvents(typeof(ApplicationExtensions).Assembly);
        services.AddHttpClient();
        // services.AddScoped<IAppService, AppService>();
        
        return services;
    }
}

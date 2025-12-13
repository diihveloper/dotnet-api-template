using DiihTemplate.Core;
using Microsoft.Extensions.DependencyInjection;

namespace DiihTemplate.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddDiihTemplateApplicationServices(this IServiceCollection services)
    {
        services.AddDiihTemplateCoreEvents(typeof(ApplicationExtensions).Assembly);
        services.AddHttpClient();
        services.AddAutoMapper(typeof(ApplicationExtensions).Assembly);
        return services;
    }
}
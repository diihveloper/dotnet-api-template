using DiihTemplate.Core;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace DiihTemplate.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddDiihTemplateApplicationServices(this IServiceCollection services)
    {
        services.AddDiihTemplateCoreEvents(typeof(ApplicationExtensions).Assembly);
        services.AddHttpClient();
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(ApplicationExtensions).Assembly);
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        return services;
    }
}
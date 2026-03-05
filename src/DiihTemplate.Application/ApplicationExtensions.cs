using DiihTemplate.Core;
using FluentValidation;
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

        // Mapster
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(ApplicationExtensions).Assembly);
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        // FluentValidation - auto-register validators from this assembly
        services.AddValidatorsFromAssembly(typeof(ApplicationExtensions).Assembly, includeInternalTypes: true);

        return services;
    }
}
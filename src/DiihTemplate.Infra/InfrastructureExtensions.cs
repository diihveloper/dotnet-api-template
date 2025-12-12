using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiihTemplate.Infra;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddDiihTemplateInfra(this IServiceCollection services,
        IConfiguration configuration)
    {
        // services.Configure<Settings>(configuration.GetSection("Settings"));
        return services;
    }
}
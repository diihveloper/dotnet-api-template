using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
#if SMTP
using DiihTemplate.Core.Services;
using DiihTemplate.Infra.Email;
#endif
#if RABBITMQ
using DiihTemplate.Core.Services;
using DiihTemplate.Infra.Messaging;
#endif
#if STORAGE
using DiihTemplate.Core.Services;
using DiihTemplate.Infra.Storage;
#endif

namespace DiihTemplate.Infra;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddDiihTemplateInfra(this IServiceCollection services,
        IConfiguration configuration)
    {
#if SMTP
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
        services.AddScoped<IEmailSender, SmtpEmailSender>();
#endif
#if RABBITMQ
        services.Configure<RabbitMqSettings>(configuration.GetSection(RabbitMqSettings.SectionName));
        services.AddSingleton<IMessageBroker, RabbitMqMessageBroker>();
#endif
#if STORAGE
        services.Configure<StorageSettings>(configuration.GetSection(StorageSettings.SectionName));
        services.AddScoped<IFileStorage, LocalFileStorage>();
#endif
        return services;
    }
}
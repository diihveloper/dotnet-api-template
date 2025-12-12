using System.Reflection;
using System.Threading.Channels;
using DiihTemplate.Core.Events;
using DiihTemplate.Core.Repositories;
using DiihTemplate.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DiihTemplate.Core;

public static class ServiceCollectionExtension
{
    public static void AddDiihTemplateCore(this IServiceCollection services)
    {
        services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddSingleton<Channel<IApplicationEvent>>(_ => Channel
            .CreateUnbounded<IApplicationEvent>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false,
                AllowSynchronousContinuations = false,
            }));
        services.AddHostedService<ApplicationEventService>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped(typeof(ICurrentUserService<>), typeof(CurrentUserService<>));

        #region Generic Repositories

        services.AddScoped(typeof(IReadOnlyRepository<>), typeof(ReadOnlyRepository<>));
        services.AddScoped(typeof(IReadOnlyRepository<,>), typeof(ReadOnlyRepository<,>));
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        #endregion

        #region Generic Services

        services.AddScoped(typeof(IReadOnlyAppService<,>), typeof(ReadOnlyAppService<,>));
        services.AddScoped(typeof(IReadOnlyAppService<,,,>), typeof(ReadOnlyAppService<,,,>));
        services.AddScoped(typeof(IReadOnlyAppService<,,,,>), typeof(ReadOnlyAppService<,,,,>));
        services.AddScoped(typeof(ICrudAppService<,,>), typeof(CrudAppService<,,>));
        services.AddScoped(typeof(ICrudAppService<,,,>), typeof(CrudAppService<,,,>));
        services.AddScoped(typeof(ICrudAppService<,,,,>), typeof(CrudAppService<,,,,>));
        services.AddScoped(typeof(ICrudAppService<,,,,,>), typeof(CrudAppService<,,,,,>));

        #endregion
    }

    public static void AddDiihTemplateCoreEvents(this IServiceCollection services, Assembly assembly)
    {
        services.AddSingleton<Channel<IApplicationEvent>>(_ => Channel
            .CreateUnbounded<IApplicationEvent>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false,
                AllowSynchronousContinuations = false,
            }));
        services.AddHostedService<ApplicationEventService>();
        var baseType = typeof(IApplicationEventHandler<>);
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract &&
                        t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == baseType))
            .ToList();

        foreach (var t in types)
        {
            var @interface = t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == baseType)
                .First().GenericTypeArguments.First();
            var handlerType = baseType.MakeGenericType(@interface);
            services.AddScoped(handlerType, t);
        }
    }
}
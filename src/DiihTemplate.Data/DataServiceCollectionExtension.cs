using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DiihTemplate.Data;

public static class DataServiceCollectionExtension
{
    public static IServiceCollection AddDiihTemplateDbContext(this IServiceCollection services, string? connectionString = null)
    {
        return services.AddDbContextPool<DbContext, DiihTemplateDbContext>(opt =>
        {
            opt.UseLazyLoadingProxies();
#if POSTGRES
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("Connection string not found");
            opt.UseNpgsql(connectionString, npgsql =>
                npgsql.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null));
#elif SQLSERVER
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("Connection string not found");
            opt.UseSqlServer(connectionString, sqlServer =>
                sqlServer.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null));
#else
            opt.UseInMemoryDatabase("DiihTemplateMemoryDatabase");
#endif
        }).AddRepositories();
    }


    static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services;
    }
}
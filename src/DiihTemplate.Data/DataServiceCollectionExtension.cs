using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DiihTemplate.Data;

public static class DataServiceCollectionExtension
{
    public static IServiceCollection AddDiihTemplateDbContext(this IServiceCollection services, string? database,
        string? connectionString)
    {
        return services.AddDbContextPool<DbContext, DiihTemplateDbContext>(opt =>
        {
            if (string.IsNullOrEmpty(database))
                throw new Exception("Database not found");
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("Connection string not found");
            opt.UseLazyLoadingProxies();
#if POSTGRES
            opt.UseNpgsql(connectionString);
#elif SQLSERVER
        opt.UseSqlServer(connectionString);
#else
#error Nenhum banco de dados definido! Use POSTGRES ou SQLSERVER.
#endif
        }).AddRepositories();
    }


    static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services;
    }
}
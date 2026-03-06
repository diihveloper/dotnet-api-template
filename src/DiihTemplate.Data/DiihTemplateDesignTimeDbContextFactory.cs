using DiihTemplate.Core.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DiihTemplate.Data;

public class DiihTemplateDesignTimeDbContextFactory : IDesignTimeDbContextFactory<DiihTemplateDbContext>
{
    public DiihTemplateDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DiihTemplateDbContext>();
        optionsBuilder.UseLazyLoadingProxies();

#if POSTGRES
        optionsBuilder.UseNpgsql("Host=localhost;Database=design_time");
#elif SQLSERVER
        optionsBuilder.UseSqlServer("Server=localhost;Database=design_time;TrustServerCertificate=True");
#else
        throw new InvalidOperationException(
            "No database provider configured. This project must be generated with --DATABASE Postgres or --DATABASE SqlServer.");
#endif

        return new DiihTemplateDbContext(optionsBuilder.Options, new NoOpDomainEventDispatcher(),
            new HttpContextAccessor());
    }

    private class NoOpDomainEventDispatcher : IDomainEventDispatcher
    {
        public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents) => Task.CompletedTask;
    }
}

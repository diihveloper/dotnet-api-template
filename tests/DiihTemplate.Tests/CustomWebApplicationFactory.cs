using DiihTemplate.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DiihTemplate.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DiihTemplateDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContext));
            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            // Add in-memory database for tests
            services.AddDbContext<DbContext, DiihTemplateDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
                options.UseLazyLoadingProxies();
            });
        });

        builder.UseEnvironment("Development");
    }
}

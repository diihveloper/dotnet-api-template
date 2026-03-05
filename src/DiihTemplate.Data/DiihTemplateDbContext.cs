using System.Security.Claims;
using DiihTemplate.Core.Entities;
using DiihTemplate.Core.Events;
using DiihTemplate.Core.Utils;
using DiihTemplate.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DiihTemplate.Data;

public class DiihTemplateDbContext : IdentityDbContext<AppUser>
{
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private IHttpContextAccessor _httpContextAccessor;

    public DiihTemplateDbContext(DbContextOptions options, IDomainEventDispatcher domainEventDispatcher,
        IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _domainEventDispatcher = domainEventDispatcher;
        _httpContextAccessor = httpContextAccessor;
        this.SetupCoreEvents();
    }

    protected DiihTemplateDbContext(IDomainEventDispatcher domainEventDispatcher,
        IHttpContextAccessor httpContextAccessor)
    {
        _domainEventDispatcher = domainEventDispatcher;
        _httpContextAccessor = httpContextAccessor;
        this.SetupCoreEvents();
    }
    
    private async Task DispatchDomainEventsAsync(Func<IDomainEvent, bool> filter)
    {
        var domainEntities = ChangeTracker
            .Entries<IHasEvents>()
            .Where(x => x.Entity.Events.Any())
            .ToList();
        foreach (var entity in domainEntities)
        {
            var events = entity.Entity.Events.Where(filter).ToList();
            if (events.Count == 0) continue;
            await _domainEventDispatcher.DispatchAsync(events);
        }
    }

    private async Task ClearDomainEvents()
    {
        var domainEntities = ChangeTracker
            .Entries<IHasEvents>()
            .Where(x => x.Entity.Events.Any())
            .ToList();
        foreach (var entity in domainEntities)
        {
            entity.Entity.ClearEvents();
        }
    }
    private void UpdateAuditableEntities()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity is not { IsAuthenticated: true }) return;

        var c = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (c == null) return;
        ChangeTracker
            .Entries<IHasUpdater<string>>()
            .Where(x => x.State is EntityState.Modified or EntityState.Added)
            .ToList().ForEach(x => x.Entity.UpdaterId = c.Value);
        ChangeTracker
            .Entries<IHasCreator<string>>()
            .Where(x => x.State == EntityState.Added)
            .ToList().ForEach(x => x.Entity.CreatorId = c.Value);
        ChangeTracker
            .Entries<IHasDeleter<string>>()
            .Where(x =>
                x.State == EntityState.Modified &&
                // IsDeleted = false => IsDeleted = true
                !x.OriginalValues.GetValue<bool>("IsDeleted") && x.CurrentValues.GetValue<bool>("IsDeleted")
            )
            .ToList().ForEach(x => x.Entity.DeleterId = c.Value);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DiihTemplateDbContext).Assembly);

        // Configurar todas as propriedades DateTime para trabalhar com UTC no PostgreSQL
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(
                        new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(
                        new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime?, DateTime?>(
                            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v,
                            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v));
                }
            }
        }

        this.ModelCreatingCore<AppUser>(modelBuilder);
    }
    
    
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        throw new NotSupportedException(
            "Synchronous SaveChanges is not supported. Use SaveChangesAsync instead to avoid potential deadlocks.");
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();

        // Pre-save events run in the same transaction
        await DispatchDomainEventsAsync(e => e is IPreSaveDomainEvent);

        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        // Post-save events run after commit
        await DispatchDomainEventsAsync(e => e is not IPreSaveDomainEvent);

        await ClearDomainEvents();
        return result;
    }
}
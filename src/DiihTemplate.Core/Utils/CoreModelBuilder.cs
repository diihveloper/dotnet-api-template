using System.Linq.Expressions;
using DiihTemplate.Core.Converters;
using DiihTemplate.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DiihTemplate.Core.Utils;

public static class CoreModelBuilder
{
    private static void ConfigureAuditableUserRelationship(ModelBuilder modelBuilder, Type userType, Type type,
        string userKey, string? navigationName = null, string? collection = null)
    {
        modelBuilder.Entity(type, builder =>
        {
            builder.HasOne(userType, navigationName)
                .WithMany(collection)
                .HasForeignKey(userKey)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static List<Type> GetAssignableTypesInterfaces(this DbContext dbContext, Type type)
    {
        var self = dbContext.GetType();
        return self.GetProperties()
            .Where(p => p.PropertyType.IsGenericType &&
                        p.PropertyType.GetGenericTypeDefinition() ==
                        typeof(DbSet<>))
            .Select(x => x.PropertyType.GetGenericArguments().First())
            .Where(p => type.IsAssignableFrom(p))
            .ToList();
    }

    public static void SetupCoreEvents(this DbContext dbContext)
    {
        dbContext.ChangeTracker.StateChanged += UpdateTimestamps;
        dbContext.ChangeTracker.Tracked += UpdateTimestamps;
    }

    public static void ModelCreatingCore<TUser>(this DbContext dbContext, ModelBuilder modelBuilder)
    {
        dbContext.ConfigureAuditable(modelBuilder);
        dbContext.ConfigureFullAuditable<TUser>(modelBuilder);
        dbContext.ConfigureMetadata(modelBuilder);
    }

    static void ConfigureFullAuditable<TUser>(this DbContext dbContext, ModelBuilder modelBuilder)
    {
        var userType = typeof(TUser);

        var hasCreatorUsers = dbContext.GetAssignableTypesInterfaces(typeof(IHasCreator<>));
        var hasCreatorUsersRef = dbContext.GetAssignableTypesInterfaces(typeof(IHasCreator<,>));
        var hasUpdaters = dbContext.GetAssignableTypesInterfaces(typeof(IHasUpdater<>));
        var hasUpdatersRef = dbContext.GetAssignableTypesInterfaces(typeof(IHasUpdater<,>));
        var hasDeleters = dbContext.GetAssignableTypesInterfaces(typeof(IHasDeleter<>));
        var hasDeletersRef = dbContext.GetAssignableTypesInterfaces(typeof(IHasDeleter<,>));


        foreach (var hasCreatorUser in hasCreatorUsers)
        {
            ConfigureAuditableUserRelationship(modelBuilder, userType, hasCreatorUser, "CreatorId");
        }

        foreach (var hasCreatorUserRef in hasCreatorUsersRef)
        {
            ConfigureAuditableUserRelationship(modelBuilder, userType, hasCreatorUserRef, "CreatorId", "Creator");
        }

        foreach (var hasUpdater in hasUpdaters)
        {
            ConfigureAuditableUserRelationship(modelBuilder, userType, hasUpdater, "UpdaterId");
        }

        foreach (var hasUpdaterRef in hasUpdatersRef)
        {
            ConfigureAuditableUserRelationship(modelBuilder, userType, hasUpdaterRef, "UpdaterId", "Updater");
        }

        foreach (var hasDeleter in hasDeleters)
        {
            ConfigureAuditableUserRelationship(modelBuilder, userType, hasDeleter, "DeleterId");
        }

        foreach (var hasDeleterRef in hasDeletersRef)
        {
            ConfigureAuditableUserRelationship(modelBuilder, userType, hasDeleterRef, "DeleterId", "Deleter");
        }
    }

    static void ConfigureAuditable(this DbContext dbContext, ModelBuilder modelBuilder)
    {
        var softDeleteTypes = dbContext.GetAssignableTypesInterfaces(typeof(ISoftDelete));
        foreach (var softDeleteType in softDeleteTypes)
        {
            modelBuilder.Entity(softDeleteType, builder =>
            {
                var entity = Expression.Parameter(softDeleteType, "e");
                var property = Expression.Property(entity, "IsDeleted");
                var notDeleted = Expression.Equal(property, Expression.Constant(false));
                var lambda = Expression.Lambda(notDeleted, entity);
                builder.HasQueryFilter(lambda);
            });
        }
    }

    static void ConfigureMetadata(this DbContext dbContext, ModelBuilder modelBuilder)
    {
        var metadataTypes = dbContext.GetAssignableTypesInterfaces(typeof(IHasMetadata));
        foreach (var metadataType in metadataTypes)
        {
            modelBuilder.Entity(metadataType,
                builder =>
                {
                    builder.Property<Dictionary<string, string>>("Metadata")
                        .HasConversion<DictionaryToJson<string, string>>();
                });
        }
    }

    public static void UpdateTimestamps(object? sender, EntityEntryEventArgs e)
    {
        if (e.Entry is { Entity: IHasCreationTime newEntity, State: EntityState.Added })
        {
            newEntity.CreatedAt = DateTime.UtcNow;
        }

        if (e.Entry is { Entity: IHasUpdatedTime updatedEntity, State: EntityState.Modified or EntityState.Added })
        {
            updatedEntity.UpdatedAt = DateTime.UtcNow;
        }

        if (e.Entry is { Entity: IHasDeletedTime deletedEntity, State: EntityState.Deleted })
        {
            deletedEntity.DeletedAt = DateTime.UtcNow;
            deletedEntity.IsDeleted = true;
            e.Entry.State = EntityState.Modified;
        }

        if (e.Entry is { Entity: ISoftDelete softDeletedEntity, State: EntityState.Deleted })
        {
            softDeletedEntity.IsDeleted = true;
            e.Entry.State = EntityState.Modified;
        }
    }
}
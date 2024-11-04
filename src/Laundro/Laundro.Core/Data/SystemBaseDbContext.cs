using Laundro.Core.Domain.Entities;
using Laundro.Core.NodaTime;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Laundro.Core.Data;
public class SystemBaseDbContext : DbContext
{
    private readonly IClockService _clock;

    public SystemBaseDbContext(DbContextOptions<LaundroDbContext> options, IClockService clock) : base(options)
    {
        _clock = clock;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureManyToManyRelationships(modelBuilder);
        foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(t => typeof(ISoftDeletable).IsAssignableFrom(t.ClrType)))
        {
            entityType.AddISoftDeleteQueryFilter();
        }
    }

    private static void ConfigureManyToManyRelationships(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureStoreUserManyToManyRelationships();

        //foreach (var type in typeof(LaundroDbContext).Assembly.GetTypes())
        //{
        //    var attribute = type.GetCustomAttributes(false).OfType<ManyToManyEntityAttribute>().SingleOrDefault();
        //    if (attribute == null) continue;

        //    modelBuilder.Entity(type).HasKey(attribute.FirstKey, attribute.SecondKey);

        //    // To resolve the error: Cannot insert explicit value of identity column in table '<tablename>' when
        //    // IDENTITY_INSERT is set to OFF
        //    var idProperty = type.GetProperty(nameof(Entity.Id));
        //    if (idProperty != null)
        //    {
        //        modelBuilder.Entity(type)
        //            .Property(idProperty.Name).ValueGeneratedOnAddOrUpdate()
        //            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
        //    }
        //}
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        InterceptChanges();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void InterceptChanges()
    {
        ChangeTracker.ApplySoftDeleteOverride(_clock);
    }
}

using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Laundro.Core.Domain.Entities;

public class StoreUser : ISoftDeletable
{
    // Making Ids as optional here in the entity class, but they are required in the table
    // to remove a EF Core warning that was something like this:
    // Entity 'Role' has a global query filter defined and is the required end of a relationship with the entity 'StoreUser'. This may lead to unexpected results when the required entity is filtered out.
    public int? StoreId { get; set; }
    public Store? Store { get; set; }

    public int? UserId { get; set; }
    public User? User { get; set; }

    public int? RoleId { get; set; }
    public Role? Role { get; set; }

    public bool IsActive { get; set; }
    public Instant? DeActivatedOn { get; set; }
}

public static class StoreUserOnModelCreatingExtension
{
    public static ModelBuilder ConfigureStoreUserManyToManyRelationships(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StoreUser>()
            .HasKey(su => new { su.UserId, su.StoreId, su.RoleId });

        modelBuilder.Entity<StoreUser>()
            .HasOne(su => su.User)
            .WithMany(u => u.StoreUser)
            .HasForeignKey(su => su.UserId)
            .IsRequired(false);

        modelBuilder.Entity<StoreUser>()
            .HasOne(su => su.Store)
            .WithMany(s => s.StoreUser)
            .HasForeignKey(su => su.StoreId)
            .IsRequired(false);

        modelBuilder.Entity<StoreUser>()
            .HasOne(su => su.Role)
            .WithMany(r => r.StoreUser)
            .HasForeignKey(su => su.RoleId)
            .IsRequired(false);

        return modelBuilder;
    }
}
using Laundro.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Laundro.Core.Domain.Entities;

public class StoreUser
{
    public int? StoreId { get; set; }
    public Store? Store { get; set; }

    public int? UserId { get; set; }
    public User? User { get; set; }

    public int? RoleId { get; set; }
    public Role? Role { get; set; }

    public bool IsActive { get; set; }
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
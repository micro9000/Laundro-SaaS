using Laundro.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Laundro.Core.Data;

public class LaundroDbContext : DbContext
{
    public LaundroDbContext(DbContextOptions<LaundroDbContext> options) : base(options)
    {
        
    }
    public DbSet<ToDo> ToDos { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Store> Stores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureManyToManyRelationships(modelBuilder);
    }

    private static void ConfigureManyToManyRelationships(ModelBuilder modelBuilder)
    {
        foreach(var type in typeof(LaundroDbContext).Assembly.GetTypes())
        {
            var attribute = type.GetCustomAttributes(false).OfType<ManyToManyEntityAttribute>().SingleOrDefault();
            if (attribute == null) continue;

            modelBuilder.Entity(type).HasKey(attribute.FirstKey, attribute.SecondKey);

            // To resolve the error: Cannot insert explicit value of identity column in table '<tablename>' when
            // IDENTITY_INSERT is set to OFF
            var idProperty = type.GetProperty(nameof(Entity.Id));
            if (idProperty != null)
            {
                modelBuilder.Entity(type)
                    .Property(idProperty.Name).ValueGeneratedOnAddOrUpdate()
                    .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
            }
        }
    }
}

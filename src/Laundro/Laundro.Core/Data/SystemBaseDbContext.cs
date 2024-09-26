using Laundro.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laundro.Core.Data;
public class SystemBaseDbContext : DbContext
{
    public SystemBaseDbContext(DbContextOptions<LaundroDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureManyToManyRelationships(modelBuilder);
    }

    private static void ConfigureManyToManyRelationships(ModelBuilder modelBuilder)
    {
        foreach (var type in typeof(LaundroDbContext).Assembly.GetTypes())
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

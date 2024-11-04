using Laundro.Core.Domain.Entities;
using Laundro.Core.NodaTime;
using Microsoft.EntityFrameworkCore;

namespace Laundro.Core.Data;

public class LaundroDbContext : SystemBaseDbContext
{
    public LaundroDbContext(DbContextOptions<LaundroDbContext> options, IClockService clock) : base(options, clock)
    {
        
    }
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<StoreImage> StoreImages { get; set; }

    // This is only use in Unit and Integration tests
    public DbSet<StoreUser> StoreUsers { get; set; }
}

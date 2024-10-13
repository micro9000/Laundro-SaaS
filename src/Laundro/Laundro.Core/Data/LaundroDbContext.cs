using Laundro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Laundro.Core.Data;

public class LaundroDbContext : SystemBaseDbContext
{
    public LaundroDbContext(DbContextOptions<LaundroDbContext> options) : base(options)
    {
        
    }
    public DbSet<ToDo> ToDos { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Store> Stores { get; set; }

    // This is only use in Unit and Integration tests
    public DbSet<StoreUser> StoreUsers { get; set; }
}

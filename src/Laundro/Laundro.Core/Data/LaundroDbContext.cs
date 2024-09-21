using Laundro.Core.Models;
using Microsoft.EntityFrameworkCore;

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
}

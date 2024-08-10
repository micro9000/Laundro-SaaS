using Microsoft.EntityFrameworkCore;
using WebApi_Microsoft_Identity.models;

namespace WebApi_Microsoft_Identity;

public class ToDoContext : DbContext
{
    public ToDoContext(DbContextOptions<ToDoContext> options) : base(options)
    {
    }

    public DbSet<ToDo> ToDos { get; set; }
}
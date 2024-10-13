namespace Laundro.Core.Domain.Entities;
public class User : Entity
{
    public string? Email { get; set; }
    public string? Name { get; set; }
    public int RoleId { get; set; }
    public Role? Role { get; set; }

    public ICollection<StoreUser> StoreUser { get; set; } = [];
}

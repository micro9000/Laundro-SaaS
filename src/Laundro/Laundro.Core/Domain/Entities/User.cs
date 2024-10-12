namespace Laundro.Core.Domain.Entities;
public class User : Entity
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; }
    public int RoleId { get; set; }
    public Role? Role { get; set; }
}

namespace Laundro.Core.Models;
public class User : Entity
{
    public string Email { get; set; } = string.Empty;
    public ICollection<Role> Roles { get; set; } = new HashSet<Role>();
}

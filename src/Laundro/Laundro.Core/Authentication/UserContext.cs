using Laundro.Core.Domain.Models;

namespace Laundro.Core.Authentication;
public class UserContext
{
    public int UserId { get; set; }
    public string? Email { get; set; }

    public Tenant? Tenant { get; set; }
    public List<Role>? Roles { get; set; }

    //[JsonInclude]
    public List<Store>? Stores { get; set; }

    public static string Key => nameof(UserContext);
}

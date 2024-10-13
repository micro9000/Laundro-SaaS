using Laundro.Core.Domain.Entities;

namespace Laundro.Core.Features.UserContextState.Models;
public class UserContext
{
    public int UserId { get; set; }
    public string? Email { get; set; }

    public Tenant? Tenant { get; set; }
    public Role? Role { get; set; }
    public bool IsTenantOwner { get; set; } = false;

    //[JsonInclude]
    public List<Store>? Stores { get; set; }

    public static string Key => nameof(UserContext);
}

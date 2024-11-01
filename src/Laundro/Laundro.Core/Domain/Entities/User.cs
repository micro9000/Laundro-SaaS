namespace Laundro.Core.Domain.Entities;
public class User : Entity
{
    public string? Email { get; set; }
    public string? Name { get; set; }
    public int RoleId { get; set; }
    public Role? Role { get; set; } = null;

    public int? CreatedInTenantId { get; set; }

    // These should not be nullable
    // To avoid this error: CS8620 - Argument cannot be used for parameter due to differences in the nullability of reference types.
    public ICollection<StoreUser> StoreUser { get; set; } = new List<StoreUser>();
}

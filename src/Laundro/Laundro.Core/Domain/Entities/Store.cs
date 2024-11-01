namespace Laundro.Core.Domain.Entities;
public class Store : Entity
{
    public string? Name { get; set; }
    public string? Location { get; set; }

    public int TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    public ICollection<StoreImage>? Images { get; set; }

    // These should not be nullable
    // To avoid this error: CS8620 - Argument cannot be used for parameter due to differences in the nullability of reference types.
    public ICollection<StoreUser> StoreUser { get; set; } = new List<StoreUser>();
}

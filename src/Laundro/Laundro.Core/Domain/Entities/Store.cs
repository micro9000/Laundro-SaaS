namespace Laundro.Core.Domain.Entities;
public class Store : Entity
{
    public string? Name { get; set; }

    public int TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    public ICollection<StoreUser> StoreUser { get; set; } = [];
}

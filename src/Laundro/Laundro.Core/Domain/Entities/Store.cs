using Laundro.Core.Data;

namespace Laundro.Core.Domain.Entities;
public class Store : Entity
{
    public string Name { get; set; } = string.Empty;
    public int ManagerId { get; set; }
    public User? Manager { get; set; }

    public int TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    public ICollection<StoreStaffAssignments>? StaffAssignments { get; set; } = new List<StoreStaffAssignments>();
}


[ManyToManyEntity(nameof(StoreId), nameof(StaffId))]
public class StoreStaffAssignments
{
    public int StoreId { get; set; }
    public Store? Store { get; set; }

    public int StaffId { get; set; }
    public User? User { get; set; }
}

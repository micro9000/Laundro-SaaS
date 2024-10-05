using Laundro.Core.Data;

namespace Laundro.Core.Domain.Models;
public class Store : Entity
{
    public string Name { get; set; } = string.Empty;
    public int ManagerId { get; set; }
    public User? Manager { get; set; }

    public int TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    public ICollection<StoreStaffAssignment> StaffAssignments { get; set; } = new List<StoreStaffAssignment>();
}


[ManyToManyEntity(nameof(StoreId), nameof(UserId))]
public class StoreStaffAssignment
{
    public int StoreId { get; set; }
    public Store? Store { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }
}

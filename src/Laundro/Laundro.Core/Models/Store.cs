using Laundro.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laundro.Core.Models;
public class Store : Entity
{
    public string Name { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public User? Owner { get; set; }

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

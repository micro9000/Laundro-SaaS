using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Laundro.Core.Authentication;
public class StoreContext
{
    public int StoreId { get; set; }
    public int OwnerId { get; set; }

    [JsonInclude]
    public List<int> StaffUserIds { get; set; }
}

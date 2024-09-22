using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Laundro.Core.Authentication;
public class UserContext
{
    public int UserId { get; set; }
    public string? Email { get; set; }
    public RoleContext? Role { get; set; }

    [JsonInclude]
    public List<StoreContext> Stores { get; set; }

    public static string Key => nameof(UserContext);
}

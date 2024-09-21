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
}

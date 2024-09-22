using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laundro.Core.Authentication;
public interface ICurrentUserAccessor
{
    UserContext? GetCurrentUser();
}

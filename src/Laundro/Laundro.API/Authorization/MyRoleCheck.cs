using Laundro.Core.Constants;
using Laundro.Core.Features.UserContextState.Models;
using System.Diagnostics.Eventing.Reader;

namespace Laundro.API.Authorization;

public static class MyRoleCheck
{
    public static bool HasAValidLaundroRole(this UserContext userContext)
    {
        var roles = new[]
        {
            Roles.new_user,
            Roles.tenant_owner,
            Roles.store_manager,
            Roles.store_staff
        };

        var hasLanudroRole = roles.Any(role => IsInRole(userContext, role.ToString()));
        return hasLanudroRole;
    }


    public static bool IsInRole(this UserContext context, string[] roleSystemKey)
    {
        return roleSystemKey.Any(r => r == context.Role?.SystemKey);
    }

    public static bool IsInRole(this UserContext context, string roleSystemKey)
    {
        return context.Role?.SystemKey == roleSystemKey;
    }
}

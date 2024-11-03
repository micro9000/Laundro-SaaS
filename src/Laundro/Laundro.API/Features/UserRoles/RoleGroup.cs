using FastEndpoints;

namespace Laundro.API.Features.UserRoles;

public class RoleGroup : Group
{
    public RoleGroup()
    {
        Configure("role", ep =>
        {
            ep.Description(x => x.Produces(401).WithTags("Role"));
        });
    }
}

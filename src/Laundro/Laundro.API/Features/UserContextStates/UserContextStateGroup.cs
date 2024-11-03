using FastEndpoints;

namespace Laundro.API.Features.UserContextStates;

public class UserContextStateGroup : Group
{
    public UserContextStateGroup()
    {
        Configure("user-context-state", ep =>
        {
            ep.Description(x => x.Produces(401).WithTags("UserContextState"));
        });
    }
}

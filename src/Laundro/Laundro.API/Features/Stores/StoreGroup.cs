using FastEndpoints;

namespace Laundro.API.Features.Stores;

public class StoreGroup : Group
{
    public StoreGroup()
    {
        Configure("store", ep =>
        {
            ep.Description(x => x.Produces(401).WithTags("Store"));
        });
    }
}

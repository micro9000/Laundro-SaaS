using FastEndpoints;

namespace Laundro.API.Features.Stores.StoreImages;

public class StoreImagesGroup : SubGroup<StoreGroup>
{
    public StoreImagesGroup()
    {
        Configure("images", ep =>
        {
            ep.Description(x => x.Produces(401).WithTags("store_images"));
        });
    }
}

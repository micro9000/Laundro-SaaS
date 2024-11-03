using FastEndpoints;

namespace Laundro.API.Features.Tenants;

public class TenantGroup : Group
{
    public TenantGroup()
    {
        Configure("tenant", ep =>
        {
            ep.Description(x => x.Produces(401).WithTags("Tenant"));
        });
    }
}

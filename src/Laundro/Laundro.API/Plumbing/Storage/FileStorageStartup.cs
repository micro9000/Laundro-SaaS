
using Laundro.Core.Data;
using Laundro.Core.Features.Stores.ProfileStorage;
using Microsoft.EntityFrameworkCore;

namespace Laundro.API.Plumbing.Storage;

public class FileStorageStartup : IStartupService
{
    private readonly IStoreProfileImagesStorage _storeProfileImagesStorage;
    private readonly LaundroDbContext _laundroDbContext;

    public FileStorageStartup(LaundroDbContext laundroDbContext, 
        IStoreProfileImagesStorage storeProfileImagesStorage)
    {
        _storeProfileImagesStorage = storeProfileImagesStorage;
        _laundroDbContext = laundroDbContext;
    }

    public async Task Initialize(CancellationToken cancellation)
    {
        // We can moved these logic in a Queue triggered Azure Function upon creation of Tenant
        // but for now, we are going to create the Tenant own container here
        
        var tenants = await _laundroDbContext.Tenants.IgnoreQueryFilters().Where(x => x.IsActive).ToListAsync();

        foreach(var tenant in tenants)
        {
            await _storeProfileImagesStorage.EnsureTenantContainerExists(tenant.TenantGuid.ToString());
        }
    }
}

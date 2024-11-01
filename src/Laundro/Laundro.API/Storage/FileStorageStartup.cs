using Laundro.Core.Constants;
using Laundro.Core.Data;
using Laundro.Core.Features.Stores.ProfileStorage;
using Microsoft.EntityFrameworkCore;

namespace Laundro.API.Storage;

public class FileStorageStartup : IStartupService
{
    private readonly IStoreProfileImagesStorage _storeProfileImagesStorage;

    public FileStorageStartup(
        IStoreProfileImagesStorage storeProfileImagesStorage)
    {
        _storeProfileImagesStorage = storeProfileImagesStorage;
    }

    public async Task Initialize(CancellationToken cancellation)
    {
          await _storeProfileImagesStorage.EnsureTenantContainerExists();
    }
}

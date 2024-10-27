using Laundro.Core.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laundro.Core.Features.Stores.ProfileStorage;

public interface IStoreProfileImagesStorage
{
    Task EnsureTenantContainerExists(string tenantGuidId);
}

public class StoreProfileImagesStorage : IStoreProfileImagesStorage
{
    private readonly IAzureBlobClient _blobServiceClient;
    private readonly ILogger<StoreProfileImagesStorage> _logger;

    public StoreProfileImagesStorage(IAzureBlobClient blobServiceClient, ILogger<StoreProfileImagesStorage> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
    }

    public async Task EnsureTenantContainerExists(string tenantGuidId)
    {
        await _blobServiceClient.EnsureContainerExists($"tenant-{tenantGuidId}");
    }
}

using Laundro.Core.Storage;
using Microsoft.Extensions.Logging;

namespace Laundro.Core.Features.Stores.ProfileStorage;

public interface IStoreProfileImagesStorage
{
    Task EnsureTenantContainerExists(Guid tenantGuid);
    Task<string> Store(Guid tenantGuid, InputFileStorageInformation fileInfo, byte[] fileContent);
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

    public async Task EnsureTenantContainerExists(Guid tenantGuid)
    {
        await _blobServiceClient.EnsureContainerExists(GetContainerName(tenantGuid.ToString()));
    }

    public async Task<string> Store(Guid tenantGuid, InputFileStorageInformation fileInfo, byte[] fileContent)
    {
        var path = FilePaths.GenerateStoreProfileFilePath(fileInfo);

        await _blobServiceClient.Store(GetContainerName(tenantGuid.ToString()), path, fileContent);

        return path;
    }

    private string GetContainerName(string tenantGuid) => $"tenant-{tenantGuid}";
}

using Laundro.Core.Constants;
using Laundro.Core.Storage;
using Microsoft.Extensions.Logging;

namespace Laundro.Core.Features.Stores.ProfileStorage;

public interface IStoreProfileImagesStorage
{
    Task EnsureTenantContainerExists();
    Task<string> Store(InputFileStorageInformation fileInfo, byte[] fileContent);
    Task<Stream> GetStoreImage(string path, CancellationToken ct);
}

public class StoreProfileImagesStorage : IStoreProfileImagesStorage
{
    private readonly IAzureBlobClient _blobServiceClient;
    private readonly ILogger<StoreProfileImagesStorage> _logger;
    private const string _containerName = TenantStorageConstants.TenantFilesContainer;

    public StoreProfileImagesStorage(IAzureBlobClient blobServiceClient, ILogger<StoreProfileImagesStorage> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
    }

    public async Task EnsureTenantContainerExists()
    {
        await _blobServiceClient.EnsureContainerExists(_containerName);
    }

    public async Task<string> Store(InputFileStorageInformation fileInfo, byte[] fileContent)
    {
        var path = FilePaths.GenerateStoreProfileFilePath(fileInfo);

        await _blobServiceClient.Store(_containerName, path, fileContent);

        return path;
    }

    public async Task<Stream> GetStoreImage(string path, CancellationToken ct)
    {
        var stream = await _blobServiceClient.ReadContent(_containerName, path, ct);
        return stream;
    }

}

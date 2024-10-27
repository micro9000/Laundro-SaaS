using Azure.Storage.Blobs.Models;

namespace Laundro.Core.BlobStorage;
public interface IAzureBlobClient
{
    Task Store(string containerName, string path, Stream fileStream);
    Task Store(string containerName, string path, byte[] fileContent, BlobUploadOptions? options = null);

    Task<Stream> ReadContent(string containerName, string path, CancellationToken cancellationToken);
    Task EnsureContainerExists(string containerName);

    Task Delete(string containerName, string path);
    Task BatchDelete(IEnumerable<Uri> paths);
    Task DeleteFilesFromContainer(string containerName, IEnumerable<string> filenames);

    Task<List<string>> ListBlobsFlatInContainer(string containerName);
    Uri GetContainerUri(string containerName);
}

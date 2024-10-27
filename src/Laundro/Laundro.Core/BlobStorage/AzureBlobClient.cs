using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Laundro.Core.BlobStorage;
public class AzureBlobClient : IAzureBlobClient
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<AzureBlobClient> _logger;

    public AzureBlobClient(BlobServiceClient blobServiceClient, ILogger<AzureBlobClient> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
    }

    public async Task BatchDelete(IEnumerable<Uri> paths)
    {
        var pathsCount = paths.Count();
        if (paths == null || pathsCount == 0)
        {
            return;
        }
        
        for (var i = 0; paths.Skip(i * 255).Take(255).Any(); i++)
        {
            // make amount in batch is 256, this just batches it accordingly
            var partitionedPaths = paths.Skip(i * 255).Take(255);

            var stringPaths = string.Join(", ", partitionedPaths);
            _logger.LogTrace("Deleting files [Paths={paths}]", stringPaths);

            try
            {
                var batch = _blobServiceClient.GetBlobBatchClient();
                await batch.DeleteBlobsAsync(partitionedPaths, DeleteSnapshotsOption.IncludeSnapshots);
                _logger.LogTrace("Batch Files successfully deleted.");
            }
            catch (AggregateException exception)
            {
                var containsNotFoundExceptions = exception.InnerExceptions
                    .Any(e => e.Message.Contains("BlobNotFound") &&
                        (e.Message.Contains("Not Found") || e.Message.Contains("Status: 404")));

                if (containsNotFoundExceptions)
                {
                    _logger.LogWarning("Blobs were unable to be found for specified paths");
                }

                var blobNotFoundErrorKeys = new string[] { "BlobNotFound", "Not Found", "Status: 404" };
                var containsOtherExceptions = exception.InnerExceptions
                    .Any(e => !blobNotFoundErrorKeys.Any(key => e.Message.Contains(key)));

                if (containsOtherExceptions)
                {
                    throw;
                }
            }
        }
    }

    public async Task Delete(string containerName, string path)
    {
        _logger.LogTrace("Deleting file [Path={path}]", path);

        var blobClient = GetBlobClient(containerName, path);
        var deleted = await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);

        _logger.LogInformation("File {status} deleted at [Path={path}]", deleted ? "successfully" : "could not be", path);
    }

    public async Task DeleteFilesFromContainer(string containerName, IEnumerable<string> filenames)
    {
        var uri = GetContainerUri(containerName);

        var itemsWithFilePaths = filenames.Select(x => new Uri($"{uri}/{x}")).Distinct();

        await BatchDelete(itemsWithFilePaths);
    }

    public async Task EnsureContainerExists(string containerName)
    {
        _logger.LogTrace("Ensuring existence of blob container [Name={containerName}]", containerName);

        var blobClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await blobClient.CreateIfNotExistsAsync();
    }

    public Uri GetContainerUri(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        return containerClient.Uri;
    }

    public async Task<List<string>> ListBlobsFlatInContainer(string containerName)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        var results = blobContainerClient.GetBlobsAsync().AsPages();
        var blobsList = new List<string>();
        await foreach (Page<BlobItem> blobPage in results)
        {
            foreach(BlobItem blobItem in blobPage.Values)
            {
                blobsList.Add(blobItem.Name);
            }
        }
        return blobsList;
    }

    public async Task<Stream> ReadContent(string containerName, string path, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Downloading file [Path={path}]", path);

        var blobClient = GetBlobClient(containerName, path);

        Stream blobContent = await blobClient.OpenReadAsync(cancellationToken: cancellationToken);

        _logger.LogInformation("File [Path={path}] successfully retrieved", path);

        return blobContent;
    }

    public async Task Store(string containerName, string path, Stream fileStream)
    {
        _logger.LogTrace("Storing file [Path={path}]", path);

        var blobClient = GetBlobClient(containerName, path);
        await blobClient.UploadAsync(fileStream);

        _logger.LogInformation("File successfully stored at [Path={path}]", path);
    }

    public async Task Store(string containerName, string path, byte[] fileContent, BlobUploadOptions? options = null)
    {
        _logger.LogTrace("Storing file [Path={path}]", path);

        var blobClient = GetBlobClient(containerName, path);
        await blobClient.UploadAsync(new BinaryData(fileContent), options);

        _logger.LogInformation("File successfully stored at [Path={path}]", path);
    }

    private BlobClient GetBlobClient(string containerName, string importFileLocation)
    {
        var blobContainer = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = blobContainer.GetBlobClient(importFileLocation);
        return blobClient;
    }
}

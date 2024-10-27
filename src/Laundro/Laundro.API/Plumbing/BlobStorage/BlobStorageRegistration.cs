using Azure.Identity;
using Azure.Storage.Blobs;
using Laundro.Core.BlobStorage;

namespace Laundro.API.Plumbing.BlobStorage;

public static class BlobStorageRegistration
{
    public static IServiceCollection AddBlobStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var blobStorageConnectionString = configuration.GetSection("AzureStorageSettings").GetValue<string>("BlobStorage");
        var managedIdentityClientId = configuration.GetSection("AzureStorageSettings").GetValue<string>("clientId");

        if (string.IsNullOrEmpty(blobStorageConnectionString))
        {
            throw new ArgumentNullException(blobStorageConnectionString);
        }

        services.AddScoped(_ => BlobServiceClientFactory(blobStorageConnectionString!, managedIdentityClientId));
        services.AddScoped<IAzureBlobClient, AzureBlobClient>();

        return services;
    }


    private static BlobServiceClient BlobServiceClientFactory(string blobStorageConnectionString, string? managedIdentityClientId)
    {
        if (blobStorageConnectionString.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            // Rely on managed identity for authentication
            return new BlobServiceClient(new Uri(blobStorageConnectionString), CreateCredential(managedIdentityClientId));
        }
        else
        {
            return new BlobServiceClient(blobStorageConnectionString);
        }
    }

    private static DefaultAzureCredential CreateCredential(string? managedIdentityClient)
    {
        DefaultAzureCredential credential;

        // And handle user-assigned managed identities too
        if (!string.IsNullOrEmpty(managedIdentityClient))
        {
            credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = managedIdentityClient });
        }
        else
        {
            credential = new DefaultAzureCredential();
        }
        return credential;
    }
}

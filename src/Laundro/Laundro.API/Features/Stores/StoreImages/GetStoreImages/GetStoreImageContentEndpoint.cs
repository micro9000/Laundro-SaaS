using FastEndpoints;
using Laundro.API.Features.Stores;
using Laundro.API.Features.Stores.GetStores;
using Laundro.Core.Data;
using Laundro.Core.Features.Stores.ProfileStorage;
using Microsoft.EntityFrameworkCore;

namespace Laundro.API.Features.Stores.StoreImages.GetStoreImages;

internal class GetStoreImageContentEndpoint : Endpoint<GetStoreImageContentRequest>
{
    private readonly LaundroDbContext _dbContext;
    private readonly IStoreProfileImagesStorage _storeProfileImagesStorage;
    private readonly ILogger<GetStoresEndpoints> _logger;

    public GetStoreImageContentEndpoint(
        LaundroDbContext dbContext,
        IStoreProfileImagesStorage storeProfileImagesStorage,
        ILogger<GetStoresEndpoints> logger)
    {
        _dbContext = dbContext;
        _storeProfileImagesStorage = storeProfileImagesStorage;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("content/{@TenantGuid}/{@StoreId}/{@ImageId}", x => new { x.tenantGuid, x.StoreId, x.ImageId });
        Group<StoreImagesGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetStoreImageContentRequest request, CancellationToken ct)
    {
        // NOTE: This endpoint is a public endpoint, always return no content if exception thrown and image info and content not found

        if (request.tenantGuid is null ||
            request.ImageId == default || request.ImageId == 0
            || request.StoreId == default || request.StoreId == 0)
        {
            _logger.LogError("Invalid arguments {@request}", request);
            await SendNoContentAsync();
        }

        try
        {
            var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(t => t.TenantGuid == Guid.Parse(request.tenantGuid!));
            if (tenant is null)
            {
                _logger.LogError("Tenant not found with this request {@request}", request);
                await SendNoContentAsync();
            }

            _logger.LogInformation("Retrieving the images for store {storeId} and tenant {tenantId}", request.StoreId, tenant!.Id);

            var storesWithImage = await _dbContext.Stores
                .Include(s => s.Images)
                .FirstOrDefaultAsync(s => s.TenantId == tenant!.Id
                    && s.Id == request.StoreId
                    && s.Images != null && s.Images.Any(i => i.Id == request.ImageId));

            if (storesWithImage != null && storesWithImage?.Images?.Count > 0)
            {
                var imageInfo = storesWithImage?.Images?.First(i => i.Id == request.ImageId);
                var imageStream = await _storeProfileImagesStorage.GetStoreImage(imageInfo?.Url!, ct);

                var fileName = "temp";
                if (imageInfo?.Filename != null)
                {
                    fileName = imageInfo.Filename;
                }
                else if (imageInfo?.Url != null)
                {
                    var tmpFilename = imageInfo?.Url?.Split('/');
                    fileName = tmpFilename?.ElementAt(tmpFilename.Length - 1);
                }

                if (imageStream != null)
                {
                    await SendStreamAsync(
                        stream: imageStream,
                        fileName: fileName,
                        fileLengthBytes: imageStream.Length,
                        contentType: imageInfo?.ContentType!);

                    return;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        await SendNoContentAsync();
    }
}

internal class GetStoreImageContentRequest
{
    public int ImageId { get; set; }
    public int StoreId { get; set; }
    public string? tenantGuid { get; set; }
}
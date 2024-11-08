using FastEndpoints;
using Laundro.API.Authorization;
using Laundro.API.Features.Stores;
using Laundro.API.Features.Stores.GetStores;
using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Features.UserContextState.Services;
using Microsoft.EntityFrameworkCore;

namespace Laundro.API.Features.Stores.StoreImages.GetStoreImages;

internal class GetStoreImagesInfoEndpoint : Endpoint<GetStoreImagesInfoRequest, GetStoreImagesInfoResponse>
{
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly LaundroDbContext _dbContext;
    private readonly ILogger<GetStoresEndpoints> _logger;

    public GetStoreImagesInfoEndpoint(
        ICurrentUserAccessor currentUserAccessor,
        LaundroDbContext dbContext,
        ILogger<GetStoresEndpoints> logger)
    {
        _currentUserAccessor = currentUserAccessor;
        _dbContext = dbContext;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("get-images-info/{@StoreId}", x => new { x.StoreId });
        Group<StoreImagesGroup>();
        Policies(PolicyName.IsTenantOwner);
    }

    public override async Task HandleAsync(GetStoreImagesInfoRequest request, CancellationToken ct)
    {
        IEnumerable<StoreImage> images = Enumerable.Empty<StoreImage>();
        try
        {
            var tenantId = _currentUserAccessor.GetCurrentUser()?.Tenant?.Id;
            _logger.LogInformation("Retrieving the images for store {storeId} and tenant {tenantId}", request.StoreId, tenantId);

            var storesWithImages = await _dbContext.Stores
                .Include(s => s.Images)
                .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Id == request.StoreId);

            if (storesWithImages is not null && storesWithImages?.Images?.Count > 0)
            {
                images = storesWithImages.Images;
            }

            _logger.LogInformation("Retrieved {imagesCount} images for store {storeId}", images.Count(), request.StoreId);

        }
        catch (Exception ex)
        {
            AddError("Unable to fetch store's images due to internal server error");
            _logger.LogError(ex, ex.Message);
            throw;
        }

        ThrowIfAnyErrors();// If there are errors, execution shouldn't go beyond this point

        await SendAsync(new GetStoreImagesInfoResponse
        {
            ImagesInfo = images,
        });
    }
}

internal class GetStoreImagesInfoRequest
{
    public int StoreId { get; set; }
}

internal class GetStoreImagesInfoResponse
{
    public IEnumerable<StoreImage>? ImagesInfo { get; set; }
}
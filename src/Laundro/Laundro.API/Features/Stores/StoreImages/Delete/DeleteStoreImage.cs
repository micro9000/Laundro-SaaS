using FastEndpoints;
using Laundro.API.Authorization;
using Laundro.API.Features.Stores.CreateStore;
using Laundro.Core.Data;
using Laundro.Core.Features.UserContextState.Services;
using Laundro.Core.NodaTime;
using Microsoft.EntityFrameworkCore;

namespace Laundro.API.Features.Stores.StoreImages.Delete;

internal class DeleteStoreImage : Endpoint<DeleteStoreImageRequest>
{
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly LaundroDbContext _dbContext;
    private readonly IClockService _clock;
    private readonly ILogger<CreateStoreEndpoint> _logger;

    public DeleteStoreImage(
        ICurrentUserAccessor currentUserAccessor,
        LaundroDbContext dbContext,
        IClockService clock,
        ILogger<CreateStoreEndpoint> logger)
    {
        _currentUserAccessor = currentUserAccessor;
        _dbContext = dbContext;
        _clock = clock;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("delete");
        Group<StoreImagesGroup>();
        Policies(PolicyName.IsTenantOwner);
    }

    public override async Task HandleAsync(DeleteStoreImageRequest request, CancellationToken ct)
    {
        var currentUser = _currentUserAccessor.GetCurrentUser();
        var tenantId = currentUser?.Tenant?.Id;

        if (tenantId == null)
        {
            _logger.LogError("Unable to delete an image due to missing tenant id in the User Context {@UserContext}", currentUser);
            ThrowError("Unable to proceed deleting an image due to internal server error");
        }

        try
        {
            // Need to check if the store is belongs to the user's tenant
            var store = await _dbContext.Stores
                .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Id == request.StoreId, ct);

            if (store == null)
            {
                ThrowError("Invalid store id");
            }

            var storeImage = await _dbContext.StoreImages
                    .FirstOrDefaultAsync(img => img.StoreId == store.Id && img.Id == request.ImageId, ct);

            if (storeImage == null)
            {
                ThrowError("Invalid image id");
            }

            _dbContext.StoreImages.Remove(storeImage);
            await _dbContext.SaveChangesAsync();

            await SendNoContentAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}

internal class DeleteStoreImageRequest
{
    public int StoreId { get; set; }
    public int ImageId { get; set; }
}
using FastEndpoints;
using Laundro.API.Authorization;
using Laundro.API.Features.Stores.GetStores;
using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Features.UserContextState.Services;

namespace Laundro.API.Features.Stores.GetStoreImages;

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
        Get("get-images-info/{StoreId}");
        Group<StoreGroup>();
        Policies(PolicyName.IsTenantOwner);
    }

    public override async Task HandleAsync(GetStoreImagesInfoRequest request, CancellationToken ct)
    {
        IEnumerable<StoreImage> images = Enumerable.Empty<StoreImage>();

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
using FastEndpoints;
using Laundro.API.Authorization;
using Laundro.API.Features.Stores.GetStoreImages;
using Laundro.API.Features.Stores.GetStores;
using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Features.UserContextState.Services;
using Laundro.Core.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Laundro.API.Features.Stores.GetStore;

internal class GetStoreEndpoint : Endpoint<GetStoreEndpointRequest, GetStoreEndpointResponse>
{
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly LaundroDbContext _dbContext;
    private readonly IIdObfuscator _idObfuscator;
    private readonly ILogger<GetStoresEndpoints> _logger;

    public GetStoreEndpoint(
        ICurrentUserAccessor currentUserAccessor,
        LaundroDbContext dbContext,
        IIdObfuscator idObfuscator,
        ILogger<GetStoresEndpoints> logger)
    {
        _currentUserAccessor = currentUserAccessor;
        _dbContext = dbContext;
        _idObfuscator = idObfuscator;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("get-store");
        Group<StoreGroup>();
        Policies(PolicyName.IsTenantOwner);
    }

    public override async Task HandleAsync(GetStoreEndpointRequest request, CancellationToken ct)
    {
        Store? store = null;

        if (request.ObfuscatedStoreId is null)
        {
            await SendNotFoundAsync();
        }

        try
        {
            var decodedStoredId = _idObfuscator.Decode(request.ObfuscatedStoreId!);

            var tenantId = _currentUserAccessor.GetCurrentUser()?.Tenant?.Id;
            _logger.LogInformation("Retrieving store with an id of {storeId} and tenant id of {tenantId}", decodedStoredId, tenantId);

            store = await _dbContext.Stores
                .Include(s => s.Tenant)
                .Include(s => s.Images)
                .Include(s => s.StoreUser).ThenInclude(ss => ss.User)
                .Include(s => s.StoreUser).ThenInclude(ss => ss.Role)
                .FirstOrDefaultAsync(store => store.Id == decodedStoredId
                && store.TenantId == tenantId);

            if (store == null) {
                await SendNotFoundAsync();
            }
            else
            {
                store.ObfuscatedId = _idObfuscator.Encode(store.Id);
            }
        }
        catch (Exception ex)
        {
            AddError("Unable to fetch store due to internal server error");
            _logger.LogError(ex, ex.Message);
            throw;
        }

        ThrowIfAnyErrors();

        await SendAsync(new GetStoreEndpointResponse
        {
            Store = store
        });
    }
}

internal class GetStoreEndpointRequest
{
    public string? ObfuscatedStoreId { get; set; }
}

internal class GetStoreEndpointResponse
{
    public Store? Store { get; set; }
}
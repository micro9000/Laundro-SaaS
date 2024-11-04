using Azure.Core;
using FastEndpoints;
using Laundro.API.Authorization;
using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Features.UserContextState.Services;
using Laundro.Core.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Laundro.API.Features.Stores.GetStores;

internal class GetStoresEndpoints : EndpointWithoutRequest<GetStoresResponse>
{
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly LaundroDbContext _dbContext;
    private readonly IIdObfuscator _idObfuscator;
    private readonly ILogger<GetStoresEndpoints> _logger;

    public GetStoresEndpoints(
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
        Get("get-all-stores");
        Group<StoreGroup>();
        Policies(PolicyName.IsTenantOwner);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        IEnumerable<Store> stores = Enumerable.Empty<Store>();
        
        try
        {
            var tenantId = _currentUserAccessor.GetCurrentUser()?.Tenant?.Id;
            _logger.LogInformation("Retrieving all stores for tenant {tenantId}", tenantId);

            if (tenantId != null)
            {
                stores = await _dbContext.Stores
                    .Include(s => s.StoreUser).ThenInclude(su => su.User)
                    .Include(s => s.Images)
                    .Where(s => s.TenantId == tenantId)
                    .ToListAsync(ct);

                if (stores.Any())
                {
                    foreach(var store in stores)
                    {
                        // Generate obfuscated id
                        store.ObfuscatedId = _idObfuscator.Encode(store.Id);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            AddError("Unable to fetch all stores due to internal server error");
            _logger.LogError(ex, ex.Message);
            throw;
        }

        ThrowIfAnyErrors();// If there are errors, execution shouldn't go beyond this point

        await SendAsync(new GetStoresResponse
        {
            Stores = stores
        });
    }
}

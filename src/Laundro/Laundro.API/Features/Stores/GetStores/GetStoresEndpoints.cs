using FastEndpoints;
using Laundro.API.Authorization;
using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Features.UserContextState.Services;
using Microsoft.EntityFrameworkCore;

namespace Laundro.API.Features.Stores.GetStores;

internal class GetStoresEndpoints : EndpointWithoutRequest<GetStoresResponse>
{
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly LaundroDbContext _dbContext;
    private readonly ILogger<GetStoresEndpoints> _logger;

    public GetStoresEndpoints(
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
        Get("api/store/getall");
        Policies(PolicyName.IsTenantOwner);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        IEnumerable<Store> stores = Enumerable.Empty<Store>();
        
        try
        {
            var tenantId = _currentUserAccessor.GetCurrentUser()?.Tenant?.Id;

            if (tenantId != null)
            {
                stores = await _dbContext.Stores
                    .Include(s => s.StoreUser).ThenInclude(su => su.User)
                    .Where(s => s.TenantId == tenantId)
                    .ToListAsync(ct);
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

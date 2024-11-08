using FastEndpoints;
using Laundro.API.Authorization;
using Laundro.API.Features.Stores.CreateStore;
using Laundro.Core.Data;
using Laundro.Core.Features.UserContextState.Repositories;
using Laundro.Core.Features.UserContextState.Services;
using Laundro.Core.NodaTime;
using Microsoft.EntityFrameworkCore;

namespace Laundro.API.Features.Stores.UpdateStore;

internal class UpdateStoreEndpoint : Endpoint<UpdateStoreRequest, UpdateStoreResponse>
{
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly LaundroDbContext _dbContext;
    private readonly IClockService _clock;
    private readonly IUserStoresRepository _userStoresRepository;
    private readonly ILogger<CreateStoreEndpoint> _logger;

    public UpdateStoreEndpoint(
        ICurrentUserAccessor currentUserAccessor,
        LaundroDbContext dbContext,
        IClockService clock,
        IUserStoresRepository userStoresRepository,
        ILogger<CreateStoreEndpoint> logger)
    {
        _currentUserAccessor = currentUserAccessor;
        _dbContext = dbContext;
        _clock = clock;
        _userStoresRepository = userStoresRepository;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("update-store");
        Group<StoreGroup>();
        Policies(PolicyName.IsTenantOwner);
    }

    public override async Task HandleAsync(UpdateStoreRequest request, CancellationToken cancellationToken)
    {
        //ThrowIfAnyErrors();
        var currentUser = _currentUserAccessor.GetCurrentUser();
        var tenantId = currentUser?.Tenant?.Id;

        if (tenantId == null)
        {
            _logger.LogError("Unable to update store due to missing tenant id in the User Context {@UserContext}", currentUser);
            ThrowError("Unable to proceed updating your store due to internal server error");
        }

        try
        {
            var store = await _dbContext.Stores
                .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Id == request.StoreId);

            if (store == null) 
            {
                ThrowError("Invalid store id");
            }
            else
            {
                store!.Name = request.Name;
                store!.Location = request.Location;

                await _dbContext.SaveChangesAsync();

                await _userStoresRepository.RefreshAndGetCachedStoresByTenant(currentUser!.UserId);
                await SendAsync(new()
                {
                    UpdateStore = store
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}

using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Features.UserContextState.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Laundro.Core.BusinessRequirementsValidators.TenantsRequirementsAndValidators.Create;
public class IsUserCanCreateMultipleTenantHandler
    : IBusinessRequirementHandler<UserCanOnlyOwnOneTenantRequirement, Tenant>
{
    private readonly LaundroDbContext _dbContext;
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly ILogger<IsUserCanCreateMultipleTenantHandler> _logger;

    public IsUserCanCreateMultipleTenantHandler(
        LaundroDbContext dbContext,
        ICurrentUserAccessor currentUserAccessor,
        ILogger<IsUserCanCreateMultipleTenantHandler> logger)
    {
        _dbContext = dbContext;
        _currentUserAccessor = currentUserAccessor;
        _logger = logger;
    }

    public async Task<ValidatorResponse> IsSatisfied(Tenant entity)
    {
        var response = new ValidatorResponse();
        var userId = _currentUserAccessor.GetCurrentUser()?.UserId;
        var activeTenant = await _dbContext.Tenants.Where(t => t.OwnerId == userId).ToListAsync();

        if (activeTenant is not null && activeTenant.Any())
        {
            _logger.LogWarning("The current user with an id of {UserId} is trying to create multiple tenants", userId);
            response.AddError("Support for multiple tenants is currently unavailable.");

            if (activeTenant.Count > 1)
            {
                _logger.LogError("The current user with an id of {UserId} is owning multiple tenants", userId);
            }
        }

        return response;
    }
}

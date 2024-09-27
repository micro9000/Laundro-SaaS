using Laundro.Shared.Constants;
using Microsoft.Azure.WebJobs.Extensions.AuthenticationEvents.TokenIssuanceStart;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Laundro.MicrosoftEntraId.AuthExtension.Claims;

public interface IClaimsService
{
    Task<WebJobsAuthenticationEventsTokenClaim[]> GetUserClaims(string email);
}

public class ClaimsService : IClaimsService
{
    private readonly IUserInfoCaching _userInfo;
    private readonly ITenantInfoCaching _tenant;
    private readonly IStoresCaching _stores;
    private readonly ILogger<ClaimsService> _logger;

    public ClaimsService(
        IUserInfoCaching userInfo,
        ITenantInfoCaching tenant,
        IStoresCaching stores,
        ILogger<ClaimsService> logger)
    {
        _userInfo = userInfo;
        _tenant = tenant;
        _stores = stores;
        _logger = logger;
    }

    public async Task<WebJobsAuthenticationEventsTokenClaim[]> GetUserClaims(string email)
    {
        var userInfo = await _userInfo.GetCachedUserInfo(email);

        var roles = new string[] { userInfo?.Role ?? nameof(Roles.new_user) };

        if (userInfo is not null) 
        {
            var tenantId = await _tenant.GetCachedTenantId(userInfo.UserId);
            if (userInfo.Role == nameof(Roles.tenant_owner))
            {
                _logger.LogWarning("User [user:@UserId] with [Tenant Owner] role doesn't have any tenant", userInfo.UserId);
            }

            var storesByManager = await _stores.GetCachedStoresByManagerId(userInfo.UserId);
            var storesByStaff = await _stores.GetCachedStoresByStaffId(userInfo.UserId);

            if (tenantId is not null && tenantId > 0) 
            {
                roles.Append(nameof(Roles.tenant_owner));
            }

            if (storesByManager is not null && storesByManager.Any())
            {
                roles.Append(nameof(Roles.store_manager));
            }

            if (storesByStaff is not null && storesByStaff.Any())
            {
                roles.Append(nameof(Roles.store_staff));
            }
        }

        var claims = new WebJobsAuthenticationEventsTokenClaim[]
        {
            new WebJobsAuthenticationEventsTokenClaim("roles", roles)
        };
        return claims;
    }
}

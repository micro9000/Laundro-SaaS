using Azure.Core;
using Laundro.Shared.Constants;
using Microsoft.Azure.WebJobs.Extensions.AuthenticationEvents.TokenIssuanceStart;
using System.Threading.Tasks;

namespace Laundro.MicrosoftEntraId.AuthExtension.Claims;

public interface IClaimsService
{
    Task<WebJobsAuthenticationEventsTokenClaim[]> GetUserClaims(string email);
}

public class ClaimsService : IClaimsService
{
    private readonly IUserInfoCaching _userInfo;

    public ClaimsService(IUserInfoCaching userInfo)
    {
        _userInfo = userInfo;
    }

    public async Task<WebJobsAuthenticationEventsTokenClaim[]> GetUserClaims(string email)
    {
        var userInfo = await _userInfo.GetCachedUserInfo(email);

        var claims = new WebJobsAuthenticationEventsTokenClaim[]
        {
            new WebJobsAuthenticationEventsTokenClaim("roles", userInfo?.role ?? nameof(Roles.new_user))
        };

        return claims;
    }
}

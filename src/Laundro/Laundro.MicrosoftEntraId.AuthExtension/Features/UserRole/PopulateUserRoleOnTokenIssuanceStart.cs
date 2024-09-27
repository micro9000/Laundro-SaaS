using Laundro.MicrosoftEntraId.AuthExtension.Claims;
using Laundro.Shared.Constants;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.AuthenticationEvents;
using Microsoft.Azure.WebJobs.Extensions.AuthenticationEvents.TokenIssuanceStart;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// https://learn.microsoft.com/en-us/entra/identity-platform/custom-extension-overview
// https://learn.microsoft.com/en-us/entra/identity-platform/custom-extension-tokenissuancestart-setup?tabs=visual-studio%2Cazure-portal&pivots=azure-portal

namespace Laundro.MicrosoftEntraId.AuthExtension.Features.UserRole;

public class PopulateUserRoleOnTokenIssuanceStart
{
    private readonly IClaimsService _claimsService;
    private readonly ILogger<PopulateUserRoleOnTokenIssuanceStart> _logger;

    public PopulateUserRoleOnTokenIssuanceStart(IClaimsService claimsService, ILogger<PopulateUserRoleOnTokenIssuanceStart> logger)
    {
        _claimsService = claimsService;
        _logger = logger;
    }

    [FunctionName("PopulateUserRoleOnTokenIssuanceStart")]
    public async Task<WebJobsAuthenticationEventResponse> Run(
        [WebJobsAuthenticationEventsTrigger()] WebJobsTokenIssuanceStartRequest request)
    {
        try
        {
            if (request.RequestStatus == WebJobsAuthenticationEventsRequestStatusType.Successful)
            {

                request.Response.Actions.Add(new WebJobsProvideClaimsForToken(
                    new WebJobsAuthenticationEventsTokenClaim("role", nameof(Roles.new_user))));

                //var userEmail = request.Data.AuthenticationContext.User.Mail;
                //var claims = await _claimsService.GetUserClaims(userEmail);

                //// additional claims
                //claims.Append(new WebJobsAuthenticationEventsTokenClaim("correlationId",
                //        request.Data.AuthenticationContext.CorrelationId.ToString()));

                //request.Response.Actions.Add(new WebJobsProvideClaimsForToken(claims));
            }
            else
            {
                _logger.LogInformation(request.StatusMessage);
            }
            return request.Completed();
        }
        catch (Exception ex)
        {
            return request.Failed(ex);
        }
    }
}

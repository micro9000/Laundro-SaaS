using Laundro.MicrosoftEntraId.AuthExtension.Claims;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.AuthenticationEvents;
using Microsoft.Azure.WebJobs.Extensions.AuthenticationEvents.TokenIssuanceStart;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

// https://learn.microsoft.com/en-us/entra/identity-platform/custom-extension-overview
// https://learn.microsoft.com/en-us/entra/identity-platform/custom-extension-tokenissuancestart-setup?tabs=visual-studio%2Cazure-portal&pivots=azure-portal

namespace Laundro.MicrosoftEntraId.AuthExtension.Features.UserRole;

public class PopulateUserRoleOnTokenIssuanceStart
{
    private readonly IClaimsService _claimsService;

    public PopulateUserRoleOnTokenIssuanceStart(IClaimsService claimsService)
    {
        _claimsService = claimsService;
    }

    [FunctionName("PopulateUserRoleOnTokenIssuanceStart")]
    public async Task<WebJobsAuthenticationEventResponse> Run(
        [WebJobsAuthenticationEventsTrigger()] WebJobsTokenIssuanceStartRequest request,
        ILogger log)
    {
        try
        {
            if (request.RequestStatus == WebJobsAuthenticationEventsRequestStatusType.Successful)
            {
                var userEmail = request.Data.AuthenticationContext.User.Mail;
                
                var claims = await _claimsService.GetUserClaims(userEmail);

                // additional claims
                claims.Append(new WebJobsAuthenticationEventsTokenClaim("correlationId",
                        request.Data.AuthenticationContext.CorrelationId.ToString()));

                request.Response.Actions.Add(new WebJobsProvideClaimsForToken(claims));
            }
            else
            {
                log.LogInformation(request.StatusMessage);
            }
            return request.Completed();
        }
        catch (Exception ex)
        {
            return request.Failed(ex);
        }
    }
}

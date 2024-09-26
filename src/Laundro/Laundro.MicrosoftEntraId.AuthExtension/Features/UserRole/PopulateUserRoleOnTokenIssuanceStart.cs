using Laundro.MicrosoftEntraId.AuthExtension.Data;
using Laundro.Shared.Constants;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.AuthenticationEvents;
using Microsoft.Azure.WebJobs.Extensions.AuthenticationEvents.TokenIssuanceStart;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

// https://learn.microsoft.com/en-us/entra/identity-platform/custom-extension-overview
// https://learn.microsoft.com/en-us/entra/identity-platform/custom-extension-tokenissuancestart-setup?tabs=visual-studio%2Cazure-portal&pivots=azure-portal

namespace Laundro.MicrosoftEntraId.AuthExtension.Features.UserRole;

public class PopulateUserRoleOnTokenIssuanceStart
{
    private readonly IUserCache _userCache;

    public PopulateUserRoleOnTokenIssuanceStart(IUserCache userCache)
    {
        _userCache = userCache;
    }

    [FunctionName("PopulateUserRoleOnTokenIssuanceStart")]
    public async Task<WebJobsAuthenticationEventResponse> Run(
        [WebJobsAuthenticationEventsTrigger()] WebJobsTokenIssuanceStartRequest request,
        ILogger log)
    {
        try
        {
            // Checks if the request is successful and did the token validation pass
            if (request.RequestStatus == WebJobsAuthenticationEventsRequestStatusType.Successful)
            {
                var userEmail = request.Data.AuthenticationContext.User.Mail;

                var userRole = await _userCache.GetUserRole(userEmail);

                request.Response.Actions.Add(new WebJobsProvideClaimsForToken(
                    new WebJobsAuthenticationEventsTokenClaim("system_role", userRole ?? nameof(Roles.new_user)),
                    new WebJobsAuthenticationEventsTokenClaim("correlationId",
                        request.Data.AuthenticationContext.CorrelationId.ToString())));
            }
            else
            {
                // If the request fails, such as in token validation, output the failed request status, 
                // such as in token validation or response validation.
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

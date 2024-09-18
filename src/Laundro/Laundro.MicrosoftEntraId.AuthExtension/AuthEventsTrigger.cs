using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.AuthenticationEvents;
using Microsoft.Azure.WebJobs.Extensions.AuthenticationEvents.TokenIssuanceStart;
using Microsoft.Extensions.Logging;
using System;

// https://learn.microsoft.com/en-us/entra/identity-platform/custom-extension-overview
// https://learn.microsoft.com/en-us/entra/identity-platform/custom-extension-tokenissuancestart-setup?tabs=visual-studio%2Cazure-portal&pivots=azure-portal

namespace Laundro.MicrosoftEntraId.AuthExtension
{
    // Reference: https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/entra/Microsoft.Azure.WebJobs.Extensions.AuthenticationEvents
    // This function project is still using .NET 6 because we are still encountering below error on .NET 8
    // [Bug] Method not found Exception when reading JWT token (Base64UrlEncoder.UnsafeDecode)
    // https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/issues/2577
    public static class AuthEventsTrigger
    {
        [FunctionName("onTokenIssuanceStart")]
        public static WebJobsAuthenticationEventResponse Run([WebJobsAuthenticationEventsTriggerAttribute(
            AudienceAppId = "<custom_authentication_extension_app_id>",
            AuthorityUrl = "<authority_uri>",
            AuthorizedPartyAppId = "<authorized_party_app_id>")] WebJobsTokenIssuanceStartRequest request, ILogger log)
        {
            try
            {
                // Checks if the request is successful and did the token validation pass
                if (request.RequestStatus == WebJobsAuthenticationEventsRequestStatusType.Successful)
                {
                    // Fetches information about the user from external data store
                    // Add new claims to the token's response
                    request.Response.Actions.Add(
                        new WebJobsProvideClaimsForToken(
                            new WebJobsAuthenticationEventsTokenClaim("dateOfBirth", "01/01/2000"),
                            new WebJobsAuthenticationEventsTokenClaim("customRoles", "Writer", "Editor"),
                            new WebJobsAuthenticationEventsTokenClaim("apiVersion", "1.0.0"),
                            new WebJobsAuthenticationEventsTokenClaim(
                                "correlationId",
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
}

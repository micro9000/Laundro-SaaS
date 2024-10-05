using Laundro.Core.Authentication;
using Laundro.Core.Data;
using Microsoft.Identity.Web;

namespace Laundro.API.Authentication;

public class UserContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserContextMiddleware> _logger;

    public UserContextMiddleware(
        RequestDelegate next,
        ILogger<UserContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext httpContext, 
        LaundroDbContext dbContext, 
        IUserAccountStateService userAccountStateService)
    {
        //var userEmail = httpContext?.User?.Identity?.Name;
        var userEmail = GetEmailClaim(httpContext);
        if (httpContext != null && !string.IsNullOrWhiteSpace(userEmail))
        {
            var userName = GetNameClaim(httpContext);
            //var userAuthId = GetObjectIdClaim(httpContext);

            var userState = await userAccountStateService.GetAndUpsertCurrentUserAccountState(userEmail, userName!);

            SetHttpContext(userState.UserContext!, httpContext);
        }

        await _next(httpContext!);
    }

    private void SetHttpContext (UserContext currentUserContext, HttpContext httpContext)
    {
        httpContext.Items[UserContext.Key] = currentUserContext;
    }

    private static string? GetEmailClaim(HttpContext httpContext) =>
        httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimConstants.PreferredUserName)?.Value;

    private static string? GetNameClaim(HttpContext httpContext) =>
        httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimConstants.Name)?.Value;

    //private static string? GetObjectIdClaim(HttpContext httpContext) =>
    //    httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimConstants.ObjectId)?.Value;
}

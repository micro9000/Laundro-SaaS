using Laundro.Core.Features.UserContextState.Models;
using Laundro.Core.Features.UserContextState.Services;
using Microsoft.AspNetCore.Authorization;

namespace Laundro.API.Authorization;

public abstract class UserAuthorizationHandler<T> : AuthorizationHandler<T> 
    where T : IAuthorizationRequirement
{
    private readonly ICurrentUserAccessor _currentUserAccessor;

    protected UserAuthorizationHandler(ICurrentUserAccessor currentUserAccessor)
    {
        _currentUserAccessor = currentUserAccessor;

        if (_currentUserAccessor is null)
        {
            throw new ArgumentNullException("_currentUserAccessor", $"{nameof(ICurrentUserAccessor)} is required");
        }
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, T requirement)
    {
        var currentUser = _currentUserAccessor.GetCurrentUser();
        if (currentUser is null)
        {
            context.Fail(new AuthorizationFailureReason(this, "Cannot process authorization requirement without an authenticated user"));
            return Task.CompletedTask;
        }

        if (currentUser.Role is null)
        {
            context.Fail(new AuthorizationFailureReason(this, "Cannot process authorization requirement on a user without a role"));
            return Task.CompletedTask;
        }

        return CheckRequirement(currentUser, context, requirement);
    }

    protected abstract Task CheckRequirement(UserContext user, AuthorizationHandlerContext context, T requirement);
}

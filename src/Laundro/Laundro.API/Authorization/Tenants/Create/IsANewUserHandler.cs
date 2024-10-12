using Laundro.Core.Constants;
using Laundro.Core.Features.UserContextState.Models;
using Laundro.Core.Features.UserContextState.Services;
using Microsoft.AspNetCore.Authorization;

namespace Laundro.API.Authorization.Tenants.Create;

public class IsANewUserHandler : UserAuthorizationHandler<HasNewUserRoleRequirement>
{
    public IsANewUserHandler(ICurrentUserAccessor currentUserAccessor) : base(currentUserAccessor){}

    private static readonly string _validRole = Roles.new_user.ToString();

    protected override Task CheckRequirement(UserContext user, AuthorizationHandlerContext context, HasNewUserRoleRequirement requirement)
    {
        if (user.IsInRole(_validRole))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        context.Fail(new AuthorizationFailureReason(this, 
            $"{user?.Role?.SystemKey} is not allowed create new tenant. The user can only create new tenant if they don't have existing tenant"));
        return Task.CompletedTask;
    }
}

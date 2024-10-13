using Laundro.Core.Constants;
using Laundro.Core.Features.UserContextState.Models;
using Laundro.Core.Features.UserContextState.Services;
using Microsoft.AspNetCore.Authorization;

namespace Laundro.API.Authorization.Tenants.Create;

public class IsInCorrectRoleHandler : UserAuthorizationHandler<HasCorrectRoleToCreateNewTeanant>
{
    public IsInCorrectRoleHandler(ICurrentUserAccessor currentUserAccessor) : base(currentUserAccessor){}

    private static readonly string[] _validRoles = new string[]
    {
        Roles.new_user.ToString(),
        Roles.tenant_owner.ToString()
    };

    protected override Task CheckRequirement(UserContext user, AuthorizationHandlerContext context, HasCorrectRoleToCreateNewTeanant requirement)
    {
        if (user.IsInRole(_validRoles))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        context.Fail(new AuthorizationFailureReason(this, 
            $"{user?.Role?.SystemKey} is not allowed create new tenant. The user can only create new tenant if they don't have existing tenant"));
        return Task.CompletedTask;
    }
}

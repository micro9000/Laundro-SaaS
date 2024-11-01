using Laundro.Core.Constants;
using Laundro.Core.Features.UserContextState.Models;
using Laundro.Core.Features.UserContextState.Services;
using Microsoft.AspNetCore.Authorization;

namespace Laundro.API.Authorization.SharedPolicy;

public class IsTenantOwnerHandler : UserAuthorizationHandler<HasTenantOwnerRole>
{
    public IsTenantOwnerHandler(ICurrentUserAccessor currentUserAccessor) : base(currentUserAccessor) { }

    protected override Task CheckRequirement(UserContext user, AuthorizationHandlerContext context, HasTenantOwnerRole requirement)
    {
        if (user.Role?.SystemKey == Roles.tenant_owner.ToString())
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        context.Fail(new AuthorizationFailureReason(this,
            $"{user?.Role?.SystemKey} is not allowed create new store."));
        return Task.CompletedTask;
    }
}

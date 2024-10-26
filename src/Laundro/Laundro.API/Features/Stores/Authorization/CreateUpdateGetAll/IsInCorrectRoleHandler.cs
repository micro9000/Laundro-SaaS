using Laundro.API.Authorization;
using Laundro.Core.Constants;
using Laundro.Core.Features.UserContextState.Models;
using Laundro.Core.Features.UserContextState.Services;
using Microsoft.AspNetCore.Authorization;

namespace Laundro.API.Features.Stores.Authorization.CreateUpdateGetAll;

public class IsInCorrectRoleHandler : UserAuthorizationHandler<HasTenantOwnerRoleToCreateStore>
{
    public IsInCorrectRoleHandler(ICurrentUserAccessor currentUserAccessor) : base(currentUserAccessor) { }

    private static readonly string[] _validRoles = new string[]
    {
        Roles.tenant_owner.ToString()
    };

    protected override Task CheckRequirement(UserContext user, AuthorizationHandlerContext context, HasTenantOwnerRoleToCreateStore requirement)
    {
        if (user.IsInRole(_validRoles))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        context.Fail(new AuthorizationFailureReason(this,
            $"{user?.Role?.SystemKey} is not allowed create new store."));
        return Task.CompletedTask;
    }
}

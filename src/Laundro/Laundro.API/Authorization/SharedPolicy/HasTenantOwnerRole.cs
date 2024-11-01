using Microsoft.AspNetCore.Authorization;

namespace Laundro.API.Authorization.SharedPolicy;

public class HasTenantOwnerRole : IAuthorizationRequirement
{
}

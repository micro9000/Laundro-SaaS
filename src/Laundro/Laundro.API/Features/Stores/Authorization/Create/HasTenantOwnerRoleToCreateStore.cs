using Microsoft.AspNetCore.Authorization;

namespace Laundro.API.Features.Stores.Authorization.Create;

public class HasTenantOwnerRoleToCreateStore : IAuthorizationRequirement
{
}

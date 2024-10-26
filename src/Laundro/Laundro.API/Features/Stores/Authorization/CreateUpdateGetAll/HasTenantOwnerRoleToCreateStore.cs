using Microsoft.AspNetCore.Authorization;

namespace Laundro.API.Features.Stores.Authorization.CreateUpdateGetAll;

public class HasTenantOwnerRoleToCreateStore : IAuthorizationRequirement
{
}

using Microsoft.AspNetCore.Authorization;

namespace Laundro.API.Authorization.Tenants.Create;

public class HasCorrectRoleToCreateNewTeanant : IAuthorizationRequirement
{
}

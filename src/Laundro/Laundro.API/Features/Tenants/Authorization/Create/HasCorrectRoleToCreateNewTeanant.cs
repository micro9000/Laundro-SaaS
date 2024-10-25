using Microsoft.AspNetCore.Authorization;

namespace Laundro.API.Features.Tenants.Authorization.Create;

public class HasCorrectRoleToCreateNewTeanant : IAuthorizationRequirement
{
}

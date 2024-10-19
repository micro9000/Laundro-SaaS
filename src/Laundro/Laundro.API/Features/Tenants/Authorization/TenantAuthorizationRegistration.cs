using Laundro.API.Authorization;
using Laundro.API.Features.Tenants.Authorization.Create;
using Microsoft.AspNetCore.Authorization;

namespace Laundro.API.Features.Tenants.Authorization;

public static class TenantAuthorizationRegistration
{
    public static IServiceCollection AddTenantAuthorizationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationHandler, IsInCorrectRoleHandler>();

        return services;
    }

    public static void AddTenantAuthorizationOptions(this AuthorizationOptions options)
    {
        options.AddPolicy(PolicyName.CanCreateTenant, policyBuilder =>
            policyBuilder.AddRequirements(
                new HasCorrectRoleToCreateNewTeanant()
            ));
    }
}

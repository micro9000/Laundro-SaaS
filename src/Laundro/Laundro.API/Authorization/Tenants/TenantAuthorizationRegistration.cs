using Laundro.API.Authorization.Tenants.Create;
using Microsoft.AspNetCore.Authorization;

namespace Laundro.API.Authorization.Tenants;

public static class TenantAuthorizationRegistration
{
    public static IServiceCollection AddTenantAuthorizationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationHandler, IsANewUserHandler>();

        return services;
    }

    public static void AddTenantAuthorizationOptions(this AuthorizationOptions options)
    {
        options.AddPolicy(PolicyName.CanCreateTenant, policyBuilder =>
            policyBuilder.AddRequirements(
                new HasNewUserRoleRequirement()
                ));
    }
}

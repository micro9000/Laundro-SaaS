using Laundro.API.Authorization.SharedPolicy;
using Laundro.API.Features.Stores.Authorization;
using Laundro.API.Features.Tenants.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Laundro.API.Authorization;

public static class AuthorizationRegistrations
{
    public static IServiceCollection AddLaundroAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        //shared policy
        services.AddScoped<IAuthorizationHandler, IsTenantOwnerHandler>();

        services.AddTenantAuthorizationServices();
        services.AddStoreAuthorizationServices();

        services.AddAuthorization(options =>
        {
            //shared policy
            options.AddPolicy(PolicyName.IsTenantOwner, policyBuilder =>
                policyBuilder.AddRequirements(new HasTenantOwnerRole()));

            options.AddTenantAuthorizationOptions();
            options.AddStoreAuthorizationOptions();
        });

        return services;
    }
}

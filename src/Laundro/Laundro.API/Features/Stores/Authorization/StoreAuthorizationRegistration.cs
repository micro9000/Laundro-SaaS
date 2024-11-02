using Laundro.API.Authorization;
using Laundro.API.Features.Stores.Authorization.CreateUpdateGetAll;
using Microsoft.AspNetCore.Authorization;

namespace Laundro.API.Features.Stores.Authorization;

public static class StoreAuthorizationRegistration
{
    // These are not being used in any endpoints for now as we introduced shared authorization policy

    public static IServiceCollection AddStoreAuthorizationServices (this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationHandler, IsInCorrectRoleHandler>();

        return services;
    }

    public static void AddStoreAuthorizationOptions (this AuthorizationOptions options)
    {
        options.AddPolicy(PolicyName.CanCreateUpdateRetrieveAllStore, policyBuilder =>
            policyBuilder.AddRequirements(
                new HasTenantOwnerRoleToCreateStore()
            ));
    }
}

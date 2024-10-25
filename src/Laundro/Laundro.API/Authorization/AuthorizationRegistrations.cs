using Laundro.API.Features.Stores.Authorization;
using Laundro.API.Features.Tenants.Authorization;

namespace Laundro.API.Authorization;

public static class AuthorizationRegistrations
{
    public static IServiceCollection AddLaundroAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTenantAuthorizationServices();
        services.AddStoreAuthorizationServices();

        services.AddAuthorization(options =>
        {
            options.AddTenantAuthorizationOptions();
            options.AddStoreAuthorizationOptions();
        });

        return services;
    }
}

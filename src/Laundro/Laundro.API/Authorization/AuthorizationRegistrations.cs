using Laundro.API.Authorization.Tenants;

namespace Laundro.API.Authorization;

public static class AuthorizationRegistrations
{
    public static IServiceCollection AddLaundroAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTenantAuthorizationServices();

        services.AddAuthorization(options =>
        {
            options.AddTenantAuthorizationOptions();
        });

        return services;
    }
}

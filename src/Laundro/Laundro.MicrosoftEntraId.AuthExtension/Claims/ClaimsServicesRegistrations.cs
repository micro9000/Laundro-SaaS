using Microsoft.Extensions.DependencyInjection;

namespace Laundro.MicrosoftEntraId.AuthExtension.Claims;
public static class ClaimsServicesRegistrations
{
    public static IServiceCollection AddClaimsServices(this IServiceCollection services)
    {
        services.AddTransient<IClaimsRepository, ClaimsRepository>();
        services.AddTransient<IUserInfoCaching, UserInfoCaching>();


        services.AddTransient<IClaimsService, ClaimsService>();

        return services;
    }
}

using Laundro.Core.Authentication.UserAccountCacheRepository;
using Microsoft.Extensions.DependencyInjection;

namespace Laundro.Core.Authentication;
public static class UserAccountStateRegistration
{
    public static IServiceCollection AddUserAccountStateServices(this IServiceCollection services)
    {
        services.AddSingleton<IUserInfoRepository, UserInfoRepository>();
        services.AddSingleton<IUserTenantRepository, UserTenantRepository>();
        services.AddSingleton<IUserStoresRepository, UserStoresRepository>();

        return services;
    }
}

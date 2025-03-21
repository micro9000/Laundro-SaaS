﻿using Microsoft.Extensions.DependencyInjection;

namespace Laundro.Core.Features.UserContextState.Repositories;
public static class UserAccountStateRegistration
{
    public static IServiceCollection AddUserAccountStateRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserInfoRepository, UserInfoRepository>();
        services.AddScoped<IUserTenantRepository, UserTenantRepository>();
        services.AddScoped<IUserStoresRepository, UserStoresRepository>();

        return services;
    }
}

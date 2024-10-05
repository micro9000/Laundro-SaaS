using Laundro.Core.Constants;
using Laundro.Core.Data;

namespace Laundro.API.Data;

public static class CachingRegistration
{
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("RedisCacheConnString");

        services.AddMemoryCache();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "LaundroInstance";
        });
        services.AddTransient<ICache, Cache>();

        return services;
    }
}
using Laundro.Core.Data;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;

namespace Laundro.API.Data;

public static class CachingRegistration
{
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("RedisCacheConnString");

        //services.AddInMemoryTokenCaches();
        services.AddMemoryCache();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "LaundroInstance";
        });
        services.AddScoped<ICache, Cache>();

        return services;
    }
}
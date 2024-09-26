using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Laundro.MicrosoftEntraId.AuthExtension.Caching;
public static class CachingServicesRegistration
{
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ICache, Cache>();

        var redisConnectionString = configuration.GetConnectionString("RedisCacheConnString");
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "LaundroInstance";
        });

        return services;
    }
}

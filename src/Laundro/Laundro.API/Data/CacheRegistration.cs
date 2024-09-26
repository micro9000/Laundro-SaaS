using Laundro.Core.Constants;
using Laundro.Core.Data;

namespace Laundro.API.Data;

public static class CachingRegistration
{
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(DistributedCacheSettings.SectionName)
            .Get<DistributedCacheSettings>() ?? new DistributedCacheSettings();
        
        services.AddMemoryCache();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = settings.ConnectionString;
            options.InstanceName = settings.InstanceName;
        });
        services.AddTransient<ICache, Cache>();

        return services;
    }
}
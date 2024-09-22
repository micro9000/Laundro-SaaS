using Laundro.Core.Constants;
using Laundro.Core.Data;

namespace Laundro.API.Data;

public static class CachingRegistration
{
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(DistributedCacheSettings.SectionName)
            .Get<DistributedCacheSettings>() ?? new DistributedCacheSettings();
        var laundroConnectionString = configuration.GetConnectionString(SystemContants.LaundroConnectionString);

        services.AddMemoryCache();
        services.AddDistributedSqlServerCache(options =>
        {
            options.ConnectionString = string.IsNullOrEmpty(settings.ConnectionString)
                ? laundroConnectionString
                : settings.ConnectionString;
            options.SchemaName = settings.SchemaName;
            options.TableName = settings.TableName;
        });
        services.AddTransient<ICache, Cache>();

        return services;
    }
}
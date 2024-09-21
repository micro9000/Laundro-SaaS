using Laundro.Core.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Laundro.API.Data;

public static class DataStorageRegistration
{
    public static IServiceCollection AddDatabaseStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("LaundroConnectionString");
        var enableSensitiveLogging = configuration.GetSection(nameof(Settings)).GetValue<bool>(nameof(Settings.EnableSensitiveLogging));

        services.AddDbContext<LaundroDbContext>(options => 
            options.UseSqlServer(connectionString, opt =>
            {
                opt.CommandTimeout(60);
            })
            .EnableSensitiveDataLogging(enableSensitiveLogging)
            .LogTo(m => Log.Logger.Information(m), LogLevel.Information));

        return services;
    }
}

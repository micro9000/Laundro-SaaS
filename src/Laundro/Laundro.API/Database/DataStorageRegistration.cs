﻿using Laundro.API.Authentication;
using Laundro.Core.Constants;
using Laundro.Core.Data;
using Laundro.Core.Features.UserContextState.Services;
using Laundro.Core.Lookups;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Laundro.API.Database;

public static class DataStorageRegistration
{
    public static IServiceCollection AddDatabaseStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(SystemContants.LaundroConnectionString);
        var enableSensitiveLogging = configuration.GetSection(nameof(Settings)).GetValue<bool>(nameof(Settings.EnableSensitiveLogging));

        services.AddDbContext<LaundroDbContext>(options =>
            options.UseSqlServer(connectionString, opt =>
            {
                opt.UseNodaTime();
                opt.CommandTimeout(60);
                opt.EnableAzureSqlRetryOnFailure();
            })
            .EnableSensitiveDataLogging(enableSensitiveLogging)
            .LogTo(m => Log.Logger.Information(m), LogLevel.Information));

        services.AddTransient<ICurrentUserAccessor, HttpUserContextAccessor>();
        services.AddTransient<IRoleLookup, RoleLookup>();

        return services;
    }
}

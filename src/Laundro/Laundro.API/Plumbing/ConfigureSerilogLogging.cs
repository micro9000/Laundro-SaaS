using Microsoft.ApplicationInsights.Extensibility;
using Serilog;

namespace Laundro.API.Plumbing;

public static class ConfigureSerilogLogging
{
    public static IServiceCollection AddSerilogLogging(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((services, loggerConfiguration) =>
                loggerConfiguration
                    .ReadFrom.Configuration(configuration)
                    .ReadFrom.Services(services)
                    .WriteTo.Console()
                    .WriteTo.ApplicationInsights(services.GetRequiredService<TelemetryConfiguration>(), TelemetryConverter.Traces));

        return services;
    }
}

using Laundro.MicrosoftEntraId.AuthExtension.Caching;
using Laundro.MicrosoftEntraId.AuthExtension.Claims;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Abstractions;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;
using System.Reflection;

[assembly: FunctionsStartup(typeof(Laundro.MicrosoftEntraId.AuthExtension.Startup))]

// Reference: https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/entra/Microsoft.Azure.WebJobs.Extensions.AuthenticationEvents
// This function project is still using .NET 6 because we are still encountering below error on .NET 8
// [Bug] Method not found Exception when reading JWT token (Base64UrlEncoder.UnsafeDecode)
// https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/issues/2577
namespace Laundro.MicrosoftEntraId.AuthExtension;
public class Startup : FunctionsStartup
{
    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        builder.ConfigurationBuilder
            .SetBasePath(builder.GetContext().ApplicationRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddUserSecrets(Assembly.GetExecutingAssembly(), true);
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        var configuration = builder.GetContext().Configuration;

        var telemetryConfiguration = new TelemetryConfiguration();

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(theme: SystemConsoleTheme.Literate)
            .WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces)
            .CreateLogger();

        builder.Services.AddLogging(logConfig =>
            logConfig.AddSerilog(Log.Logger, dispose: true)
        );

        builder.Services.AddCaching(configuration);
        builder.Services.AddClaimsServices();

    }
}
using Laundro.MicrosoftEntraId.AuthExtension.Caching;
using Laundro.MicrosoftEntraId.AuthExtension.Claims;
using Laundro.MicrosoftEntraId.AuthExtension.Data;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        builder.Services.AddCaching(configuration);
        builder.Services.AddClaimsServices();

    }
}
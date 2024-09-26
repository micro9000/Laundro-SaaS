using Laundro.MicrosoftEntraId.AuthExtension.Caching;
using Laundro.MicrosoftEntraId.AuthExtension.Data;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Reflection;

[assembly: FunctionsStartup(typeof(Laundro.MicrosoftEntraId.AuthExtension.Startup))]

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

        builder.Services.AddTransient<ICache, Cache>();
        builder.Services.AddTransient<IUserRepository, UserRepository>();
        builder.Services.AddTransient<IUserCache, UserCache>();

        var redisConnection = new RedisConnectionOptions();
        configuration.GetSection(RedisConnectionOptions.RedisConnection).Bind(redisConnection);
        builder.Services.AddSingleton<IConnectionMultiplexer>(option =>
           ConnectionMultiplexer.Connect(new ConfigurationOptions
           {
               EndPoints = { $"{redisConnection.Host}:{redisConnection.Port}" },
               AbortOnConnectFail = false,
               Ssl = redisConnection.IsSSL,
               Password = redisConnection.Password
           }));
    }
}
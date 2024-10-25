using Laundro.API.Authentication;
using Laundro.API.Plumbing;
using Laundro.Core.Constants;
using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Features.UserContextState.Services;
using Laundro.Core.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Laundro.UnitTests;
public static class TestServiceProvider
{
    public static IServiceProvider GetNewServiceProvider()
    {
        var initialData = new Dictionary<string, string?>()
        {
            {"EnableBackgroundService", "true"}
        };

        var configurationRoot = new ConfigurationBuilder()
            .AddInMemoryCollection(initialData)
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();

        // https://stackoverflow.com/a/45346358/11065063
        services.AddDbContext<LaundroDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddMemoryCache();
        services.AddDistributedMemoryCache();
        services.AddScoped<ICache, Cache>();
        services.AddTransient<ICurrentUserAccessor, HttpUserContextAccessor>();
        services.AddTransient<IRoleLookup, RoleLookup>();
        services.AddCustomNodaTimeClock();

        services.AddTransient<IRoleLookup, RoleLookup>();
        services.AddRepositories();

        return services.BuildServiceProvider();
    }

    public static void PopulateUserRoles(LaundroDbContext dbContext)
    {
        var enumRoles = Enum.GetValues(typeof(Roles)).Cast<Roles>().ToArray();
        var roles = enumRoles.Select(r => new Role { Name = r.ToString(), SystemKey = r.ToString() });
        dbContext.AddRange(roles);
        dbContext.SaveChanges();
    }
}

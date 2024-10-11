using Laundro.API.Authentication;
using Laundro.API.Plumbing;
using Laundro.Core.Constants;
using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Features.ContextState.Services;
using Laundro.Core.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Laundro.UnitTests;
public class SharedTestFixture : IDisposable
{
    public LaundroDbContext dbContext { get; private set; }
    public ILoggerFactory loggerFactory { get; private set; }
    public IServiceProvider serviceProvider { get; private set; }
    public IConfigurationRoot configurationRoot { get; private set; }

    public SharedTestFixture()
    {
        var initialData = new Dictionary<string, string?>()
        {
            {"EnableBackgroundService", "true"}
        };

        configurationRoot = new ConfigurationBuilder()
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

        serviceProvider = services.BuildServiceProvider();

        dbContext = serviceProvider.GetRequiredService<LaundroDbContext>();
        loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

        // Populate seed data
        PopulateUserRoles(dbContext);
    }

    private void PopulateUserRoles(LaundroDbContext dbContext)
    {
        var enumRoles = Enum.GetValues(typeof(Roles)).Cast<Roles>().ToArray();
        var roles = enumRoles.Select(r => new Role { Name = r.ToString(), SystemKey = r.ToString(), IsActive = true });
        dbContext.AddRange(roles);
        dbContext.SaveChanges();
    }

    public void Dispose()
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Dispose();
        loggerFactory.Dispose();
    }
}

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
using Microsoft.Extensions.Logging;

namespace Laundro.UnitTests;
public class SharedTestFixture : IDisposable
{
    public LaundroDbContext dbContext { get; private set; }
    public ILoggerFactory loggerFactory { get; private set; }
    public IServiceProvider serviceProvider { get; private set; }

    public SharedTestFixture()
    {
        serviceProvider = TestServiceProvider.GetNewServiceProvider();
        dbContext = serviceProvider.GetRequiredService<LaundroDbContext>();
        loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

        // Populate seed data
        TestServiceProvider.PopulateUserRoles(dbContext);
    }

    public void Dispose()
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Dispose();
        loggerFactory.Dispose();
    }
}

using FluentAssertions;
using Laundro.Core.Constants;
using Laundro.Core.Data;
using Laundro.Core.Lookups;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Laundro.UnitTests.Core.Lookups;
public class RoleLookupTests : IClassFixture<SharedTestFixture>
{
    private readonly SharedTestFixture _sharedTestFixture;
    private readonly RoleLookup _sut;

    public RoleLookupTests(SharedTestFixture sharedTestFixture)
    {
        _sharedTestFixture = sharedTestFixture;

        var cache = _sharedTestFixture.serviceProvider.GetRequiredService<ICache>();
        var logger = _sharedTestFixture.loggerFactory.CreateLogger<RoleLookup>();

        _sut = new RoleLookup(_sharedTestFixture.dbContext, cache, logger);
    }

    [Fact]
    public async Task When_GetNewUserRole_ItShouldReturnCorrectNewUserDbRow()
    {
        var actualDbRow = _sharedTestFixture.dbContext.Roles.FirstOrDefault(r => r.SystemKey == nameof(Roles.new_user));

        var newUserRole = await _sut.NewUser();

        actualDbRow!.Name.Should().Be(newUserRole!.Name);
    }
}

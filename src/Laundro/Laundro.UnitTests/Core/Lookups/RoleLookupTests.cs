using FluentAssertions;
using Laundro.Core.Constants;
using Laundro.Core.Data;
using Laundro.Core.Lookups;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Laundro.UnitTests.Core.Lookups;

[Collection("LookupsTestCollection")]
public class RoleLookupTests
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
    public async Task When_GetNewUserRole_ItShouldReturnCorrectDbRow()
    {
        var actualDbRow = _sharedTestFixture.dbContext.Roles.FirstOrDefault(r => r.SystemKey == nameof(Roles.new_user));

        var newUserRole = await _sut.NewUser();

        actualDbRow!.Name.Should().Be(newUserRole!.Name);
    }

    [Fact]
    public async Task When_GetTenantOwnerRole_ItShouldReturnCorrectDbRow()
    {
        var actualDbRow = _sharedTestFixture.dbContext.Roles.FirstOrDefault(r => r.SystemKey == nameof(Roles.tenant_owner));

        var newUserRole = await _sut.TenantOwner();

        actualDbRow!.Name.Should().Be(newUserRole!.Name);
    }

    [Fact]
    public async Task When_GetStoreManagerRole_ItShouldReturnCorrectDbRow()
    {
        var actualDbRow = _sharedTestFixture.dbContext.Roles.FirstOrDefault(r => r.SystemKey == nameof(Roles.store_manager));

        var newUserRole = await _sut.StoreManager();

        actualDbRow!.Name.Should().Be(newUserRole!.Name);
    }

    [Fact]
    public async Task When_GetStoreStaffRole_ItShouldReturnCorrectDbRow()
    {
        var actualDbRow = _sharedTestFixture.dbContext.Roles.FirstOrDefault(r => r.SystemKey == nameof(Roles.store_staff));

        var newUserRole = await _sut.StoreStaff();

        actualDbRow!.Name.Should().Be(newUserRole!.Name);
    }
}

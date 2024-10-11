using FluentAssertions;
using Laundro.Core.Constants;
using Laundro.Core.Features.ContextState.Models;
using Laundro.Core.Features.ContextState.Repositories;
using Laundro.Core.Features.ContextState.Services;
using Laundro.Core.Lookups;
using Laundro.Core.NodaTime;
using Microsoft.Extensions.DependencyInjection;

namespace Laundro.UnitTests.Core.Authentication;

[Collection("AuthenticationTestsCollection")]
public partial class UserAccountStateServiceTests
{
    private readonly SharedTestFixture _testFixture;
    private readonly UserAccountStateService _sut;

    public UserAccountStateServiceTests(AuthenticationTestFixture testFixture)
    {
        _testFixture = testFixture;

        var clockService = _testFixture.serviceProvider.GetRequiredService<IClockService>();
        var userInfoRepo = _testFixture.serviceProvider.GetRequiredService<IUserInfoRepository>();
        var userTenantRepo = _testFixture.serviceProvider.GetRequiredService<IUserTenantRepository>();
        var userStoresRepo = _testFixture.serviceProvider.GetRequiredService<IUserStoresRepository>();
        var roleLookup = _testFixture.serviceProvider.GetRequiredService<IRoleLookup>();

        _sut = new UserAccountStateService(
            clockService,
            _testFixture.dbContext,
            userInfoRepo,
            userTenantRepo,
            userStoresRepo,
            roleLookup);
    }

    [Fact]
    public async Task WHEN_NewUser_SignUp_and_ThereIsNoTenantYet_THEN_ItShouldSaveItsInfoAndAssignNewUserRole()
    {
        var newUserEmail = "raniel.garcia@gmail.com";
        var newUserName = "Raniel Garcia";

        var userAccountState = await _sut.GetAndUpsertCurrentUserAccountState(newUserEmail, newUserName);

        userAccountState.DbState.Should().Be(UserAccountState.UserStateInDb.Active);
        userAccountState.UserContext.Should().NotBeNull();
        userAccountState.UserContext!.Role.Should().NotBeNull();
        userAccountState.UserContext!.Role!.SystemKey.Should().Be(nameof(Roles.new_user));
        userAccountState.UserContext!.Email.Should().Be(newUserEmail);
        userAccountState.UserContext!.Stores.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task WHEN_ExistingTenantUser_SignedIn_THEN_ItShouldReturnUserContextCorrectly()
    {
        var userAccountState = await _sut.GetAndUpsertCurrentUserAccountState(TestContextData.tenantOwnerEmail, TestContextData.tenantOwnerName);

        userAccountState.DbState.Should().Be(UserAccountState.UserStateInDb.Active);
        userAccountState.UserContext.Should().NotBeNull();
        userAccountState.UserContext!.Role.Should().NotBeNull();
        userAccountState.UserContext!.Role!.SystemKey.Should().Be(nameof(Roles.tenant_owner));
        userAccountState.UserContext!.Email.Should().Be(TestContextData.tenantOwnerEmail);
        userAccountState.UserContext!.Stores.Should().NotBeEmpty();
        userAccountState.UserContext!.Stores!.Count().Should().Be(2);
    }

    [Fact]
    public async Task WHEN_ExistingStoreManagerUser_SignedIn_THEN_ItShouldReturnUserContextCorrectly()
    {
        var userAccountState = await _sut.GetAndUpsertCurrentUserAccountState(TestContextData.storeManagerEmail, TestContextData.storeManagerName);

        userAccountState.DbState.Should().Be(UserAccountState.UserStateInDb.Active);
        userAccountState.UserContext.Should().NotBeNull();
        userAccountState.UserContext!.Role.Should().NotBeNull();
        userAccountState.UserContext!.Role!.SystemKey.Should().Be(nameof(Roles.store_manager));
        userAccountState.UserContext!.Email.Should().Be(TestContextData.storeManagerEmail);
        userAccountState.UserContext!.Stores.Should().NotBeEmpty();
        userAccountState.UserContext!.Stores!.Count().Should().Be(2);
    }

    [Fact]
    public async Task WHEN_ExistingStoreStaffUser_SignedIn_THEN_ItShouldReturnUserContextCorrectly_1()
    {
        var userAccountState = await _sut.GetAndUpsertCurrentUserAccountState(TestContextData.storeStaffEmail_1, TestContextData.storeStaffName_1);

        userAccountState.DbState.Should().Be(UserAccountState.UserStateInDb.Active);
        userAccountState.UserContext.Should().NotBeNull();
        userAccountState.UserContext!.Role.Should().NotBeNull();
        userAccountState.UserContext!.Role!.SystemKey.Should().Be(nameof(Roles.store_staff));
        userAccountState.UserContext!.Email.Should().Be(TestContextData.storeStaffEmail_1);
        userAccountState.UserContext!.Stores.Should().NotBeEmpty();
        userAccountState.UserContext!.Stores!.Count().Should().Be(1);
        userAccountState.UserContext!.Stores!.First().Name.Should().Be(TestContextData.firstStore);
    }


    [Fact]
    public async Task WHEN_ExistingStoreStaffUser_SignedIn_THEN_ItShouldReturnUserContextCorrectly_2()
    {
        var userAccountState = await _sut.GetAndUpsertCurrentUserAccountState(TestContextData.storeStaffEmail_2, TestContextData.storeStaffName_2);

        userAccountState.DbState.Should().Be(UserAccountState.UserStateInDb.Active);
        userAccountState.UserContext.Should().NotBeNull();
        userAccountState.UserContext!.Role.Should().NotBeNull();
        userAccountState.UserContext!.Role!.SystemKey.Should().Be(nameof(Roles.store_staff));
        userAccountState.UserContext!.Email.Should().Be(TestContextData.storeStaffEmail_2);
        userAccountState.UserContext!.Stores.Should().NotBeEmpty();
        userAccountState.UserContext!.Stores!.Count().Should().Be(1);
        userAccountState.UserContext!.Stores!.First().Name.Should().Be(TestContextData.secondStore);
    }

}

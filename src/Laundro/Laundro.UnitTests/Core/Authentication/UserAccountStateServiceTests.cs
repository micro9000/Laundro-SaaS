using FluentAssertions;
using Laundro.Core.Constants;
using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Features.UserContextState.Models;
using Laundro.Core.Features.UserContextState.Repositories;
using Laundro.Core.Features.UserContextState.Services;
using Laundro.Core.Lookups;
using Laundro.Core.NodaTime;
using Laundro.Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;

namespace Laundro.UnitTests.Core.Authentication;

public partial class UserAccountStateServiceTests : IDisposable
{
    private readonly LaundroDbContext _dbContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly UserAccountStateService _sut;

    public UserAccountStateServiceTests()
    {
        _serviceProvider = TestServiceProvider.GetNewServiceProvider();
        _dbContext = _serviceProvider.GetRequiredService<LaundroDbContext>();

        var clockService = _serviceProvider.GetRequiredService<IClockService>();
        var userInfoRepo = _serviceProvider.GetRequiredService<IUserInfoRepository>();
        var userTenantRepo = _serviceProvider.GetRequiredService<IUserTenantRepository>();
        var userStoresRepo = _serviceProvider.GetRequiredService<IUserStoresRepository>();
        var roleLookup = _serviceProvider.GetRequiredService<IRoleLookup>();

        TestServiceProvider.PopulateUserRoles(_dbContext);
        PopulateTestData(_dbContext, roleLookup);

        _sut = new UserAccountStateService(
            clockService,
            _dbContext,
            userInfoRepo,
            userTenantRepo,
            userStoresRepo,
            roleLookup,
            new IdObfuscator());
    }

    public void Dispose()
    {
        _dbContext.Dispose();
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

        var roleLookup = _serviceProvider.GetRequiredService<IRoleLookup>();
        var storeManagerRole = await roleLookup.StoreManager();

        userAccountState.DbState.Should().Be(UserAccountState.UserStateInDb.Active);
        userAccountState.UserContext.Should().NotBeNull();
        userAccountState.UserContext!.Role.Should().NotBeNull();
        userAccountState.UserContext!.Role!.SystemKey.Should().Be(nameof(Roles.tenant_employee));
        userAccountState.UserContext!.Email.Should().Be(TestContextData.storeManagerEmail);
        userAccountState.UserContext!.Stores.Should().NotBeEmpty();
        userAccountState.UserContext!.Stores!.Count().Should().Be(2);

        userAccountState.UserContext!.Stores!
            .Where(s => s.StoreUser.Any(ss => ss.RoleId == storeManagerRole!.Id))
            .ToList().Any().Should().BeTrue();
    }

    [Fact]
    public async Task WHEN_ExistingStoreStaffUser_SignedIn_THEN_ItShouldReturnUserContextCorrectly_staff1()
    {
        var userAccountState = await _sut.GetAndUpsertCurrentUserAccountState(TestContextData.storeStaffEmail_1, TestContextData.storeStaffName_1);

        var roleLookup = _serviceProvider.GetRequiredService<IRoleLookup>();
        var storeStaff = await roleLookup.StoreStaff();

        userAccountState.DbState.Should().Be(UserAccountState.UserStateInDb.Active);
        userAccountState.UserContext.Should().NotBeNull();
        userAccountState.UserContext!.Role.Should().NotBeNull();
        userAccountState.UserContext!.Role!.SystemKey.Should().Be(nameof(Roles.tenant_employee));
        userAccountState.UserContext!.Email.Should().Be(TestContextData.storeStaffEmail_1);
        userAccountState.UserContext!.Stores.Should().NotBeEmpty();
        userAccountState.UserContext!.Stores!.Count().Should().Be(1);
        userAccountState.UserContext!.Stores!.First().Name.Should().Be(TestContextData.firstStore);

        userAccountState.UserContext!.Stores!.First()?.StoreUser?
            .Any(ss => ss.RoleId == storeStaff!.Id).Should().BeTrue();
    }


    [Fact]
    public async Task WHEN_ExistingStoreStaffUser_SignedIn_THEN_ItShouldReturnUserContextCorrectly_staff2()
    {
        var userAccountState = await _sut.GetAndUpsertCurrentUserAccountState(TestContextData.storeStaffEmail_2, TestContextData.storeStaffName_2);

        var roleLookup = _serviceProvider.GetRequiredService<IRoleLookup>();
        var storeStaff = await roleLookup.StoreStaff();

        userAccountState.DbState.Should().Be(UserAccountState.UserStateInDb.Active);
        userAccountState.UserContext.Should().NotBeNull();
        userAccountState.UserContext!.Role.Should().NotBeNull();
        userAccountState.UserContext!.Role!.SystemKey.Should().Be(nameof(Roles.tenant_employee));
        userAccountState.UserContext!.Email.Should().Be(TestContextData.storeStaffEmail_2);
        userAccountState.UserContext!.Stores.Should().NotBeEmpty();
        userAccountState.UserContext!.Stores!.Count().Should().Be(1);
        userAccountState.UserContext!.Stores!.First().Name.Should().Be(TestContextData.secondStore);

        userAccountState.UserContext!.Stores!.First()?.StoreUser?
            .Any(ss => ss.RoleId == storeStaff!.Id).Should().BeTrue();
    }

    private void PopulateTestData(LaundroDbContext dbContext, IRoleLookup roleLookup)
    {
        var tenantOwnerRole = roleLookup.TenantOwner().Result;
        var tenantEmployeeRole = roleLookup.TenantEmployee().Result;
        var storeManager = roleLookup.StoreManager().Result;
        var storeStaff = roleLookup.StoreStaff().Result;

        dbContext.Users.AddRange(new List<User>
        {
            new User
            {
                Email = TestContextData.tenantOwnerEmail,
                Name = TestContextData.tenantOwnerName,
                RoleId = tenantOwnerRole!.Id,
                IsActive = true
            },
            new User
            {
                Email = TestContextData.storeManagerEmail,
                Name = TestContextData.storeManagerName,
                RoleId = tenantEmployeeRole!.Id,
                IsActive = true
            },
            new User
            {
                Email = TestContextData.storeStaffEmail_1,
                Name = TestContextData.storeStaffName_1,
                RoleId = tenantEmployeeRole!.Id,
                IsActive = true
            },
            new User
            {
                Email = TestContextData.storeStaffEmail_2,
                Name = TestContextData.storeStaffName_2,
                RoleId = tenantEmployeeRole!.Id,
                IsActive = true
            }
        });
        dbContext.SaveChanges();

        var allUsers = dbContext.Users.ToList();

        var userTenantOwner = dbContext.Users.First(u => u.Email == TestContextData.tenantOwnerEmail);
        var userStoreManager = dbContext.Users.First(u => u.Email == TestContextData.storeManagerEmail);
        var userStoreStaff_1 = dbContext.Users.First(u => u.Email == TestContextData.storeStaffEmail_1);
        var userStoreStaff_2 = dbContext.Users.First(u => u.Email == TestContextData.storeStaffEmail_2);

        dbContext.Tenants.Add(new Tenant
        {
            OwnerId = userTenantOwner.Id,
            CreatedAt = Instant.FromDateTimeUtc(DateTime.SpecifyKind(new DateTime(2024, 10, 6), DateTimeKind.Utc)),
            TenantName = "test",
            TenantGuid = Guid.NewGuid(),
            IsActive = true
        });
        dbContext.SaveChanges();

        var tenant = dbContext.Tenants.First(t => t.OwnerId == userTenantOwner.Id);

        dbContext.Stores.AddRange(new List<Store>
        {
            new Store
            {
                TenantId = tenant.Id,
                Name = TestContextData.firstStore,
                CreatedAt = Instant.FromDateTimeUtc(DateTime.SpecifyKind(new DateTime(2024, 10, 6), DateTimeKind.Utc)),
                IsActive = true
            },
            new Store
            {
                TenantId = tenant.Id,
                Name = TestContextData.secondStore,
                CreatedAt = Instant.FromDateTimeUtc(DateTime.SpecifyKind(new DateTime(2024, 10, 6), DateTimeKind.Utc)),
                IsActive = true
            }
        });
        dbContext.SaveChanges();

        var firstStoreDb = dbContext.Stores.First(s => s.Name == TestContextData.firstStore);
        var secondStoreDb = dbContext.Stores.First(s => s.Name == TestContextData.secondStore);

        dbContext.StoreUsers.AddRange(new List<StoreUser>
        {
            new StoreUser
            {
                StoreId = firstStoreDb.Id,
                UserId = userStoreManager.Id,
                RoleId = storeManager!.Id
            },
            new StoreUser
            {
                StoreId = secondStoreDb.Id,
                UserId = userStoreManager.Id,
                RoleId = storeManager!.Id
            },
            new StoreUser
            {
                StoreId = firstStoreDb.Id,
                UserId = userStoreStaff_1.Id,
                RoleId = storeStaff!.Id
            },
            new StoreUser
            {
                StoreId = secondStoreDb.Id,
                UserId = userStoreStaff_2.Id,
                RoleId = storeStaff!.Id
            }
        });

        dbContext.SaveChanges();
    }

}

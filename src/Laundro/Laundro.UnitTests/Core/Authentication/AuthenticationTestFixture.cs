using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Laundro.Core.Lookups;
using Microsoft.Extensions.DependencyInjection;

namespace Laundro.UnitTests.Core.Authentication;
public class AuthenticationTestFixture : SharedTestFixture
{
    public AuthenticationTestFixture()
    {
        var roleLookup = serviceProvider.GetRequiredService<IRoleLookup>();

        PopulateTestData(dbContext, roleLookup);
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
            CreatedAt = new DateTime(2024, 10, 6),
            CompanyName = "test",
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
                CreatedAt = new DateTime(2024, 10, 6),
                IsActive = true
            },
            new Store
            {
                TenantId = tenant.Id,
                Name = TestContextData.secondStore,
                CreatedAt = new DateTime(2024, 10, 6),
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

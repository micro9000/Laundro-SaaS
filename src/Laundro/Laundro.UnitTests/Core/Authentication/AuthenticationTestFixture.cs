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
        var tenantRole = roleLookup.TenantOwner().Result;
        var storeManager = roleLookup.StoreManager().Result;
        var storeStaff = roleLookup.StoreStaff().Result;

        dbContext.Users.AddRange(new List<User>
        {
            new User
            {
                Email = TestContextData.tenantOwnerEmail,
                Name = TestContextData.tenantOwnerName,
                RoleId = tenantRole!.Id,
                IsActive = true
            },
            new User
            {
                Email = TestContextData.storeManagerEmail,
                Name = TestContextData.storeManagerName,
                RoleId = storeManager!.Id,
                IsActive = true
            },
            new User
            {
                Email = TestContextData.storeStaffEmail_1,
                Name = TestContextData.storeStaffName_1,
                RoleId = storeStaff!.Id,
                IsActive = true
            },
            new User
            {
                Email = TestContextData.storeStaffEmail_2,
                Name = TestContextData.storeStaffName_2,
                RoleId = storeStaff!.Id,
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
                ManagerId = userStoreManager.Id,
                CreatedAt = new DateTime(2024, 10, 6),
                IsActive = true
            },
            new Store
            {
                TenantId = tenant.Id,
                Name = TestContextData.secondStore,
                ManagerId = userStoreManager.Id,
                CreatedAt = new DateTime(2024, 10, 6),
                IsActive = true
            }
        });
        dbContext.SaveChanges();

        var firstStoreDb = dbContext.Stores.First(s => s.Name == TestContextData.firstStore);
        var secondStoreDb = dbContext.Stores.First(s => s.Name == TestContextData.secondStore);

        dbContext.StoreStaffAssignments.AddRange(new List<StoreStaffAssignments>
        {
            new StoreStaffAssignments
            {
                StoreId = firstStoreDb.Id,
                StaffId = userStoreStaff_1.Id
            },
            new StoreStaffAssignments
            {
                StoreId = secondStoreDb.Id,
                StaffId = userStoreStaff_2.Id
            }
        });
        dbContext.SaveChanges();
    }
}

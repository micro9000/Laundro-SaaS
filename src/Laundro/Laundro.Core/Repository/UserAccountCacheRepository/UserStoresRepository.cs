using Laundro.Core.Data;
using Laundro.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Laundro.Core.Repository.UserAccountCacheRepository;

public interface IUserStoresRepository
{
    Task<List<Store>?> GetCachedStoresByManagerId(int userId);
    Task<List<Store>?> GetCachedStoresByStaffId(int userId);
    void InvalidCachedStoresByManagerId(int userId);
    void InvalidCachedStoresByStaffId(int userId);
    Task<List<Store>?> RefreshAndGetCachedStoresByManagerId(int userId);
    Task<List<Store>?> RefreshAndGetCachedStoresByStaffId(int userId);

    Task<List<Store>?> GetCachedStoresByTenant(int tenantId);
    Task<List<Store>?> RefreshAndGetCachedStoresByTenant(int tenantId);
    void InvalidCachedStoresByTenant(int tenantId);
}

public class UserStoresRepository : BaseCacheService<List<Store>>, IUserStoresRepository
{
    private readonly LaundroDbContext _dbContext;

    public UserStoresRepository(ICache cache, LaundroDbContext dbContext) : base(cache)
    {
        _dbContext = dbContext;
    }

    #region Get stores by manager id
    public async Task<List<Store>?> GetCachedStoresByManagerId(int userId)
    {
        var stores = await Fetch(GetCachedKeyForManager(userId), async e =>
        {
            return await _dbContext.Stores
                .Include(s => s.Tenant)
                .AsSplitQuery()
                .Where(s => s.ManagerId == userId).ToListAsync();
        });
        return stores;
    }

    public async Task<List<Store>?> RefreshAndGetCachedStoresByManagerId(int userId)
    {
        var stores = await Refresh(GetCachedKeyForManager(userId), async e =>
        {
            return await _dbContext.Stores
                .Include(s => s.Tenant)
                .AsSplitQuery()
                .Where(s => s.ManagerId == userId).ToListAsync();
        });
        return stores;
    }

    public void InvalidCachedStoresByManagerId(int userId)
    {
        Invalidate(GetCachedKeyForManager(userId));
    }

    #endregion

    #region Get stores by staff id
    public async Task<List<Store>?> GetCachedStoresByStaffId(int userId)
    {
        var stores = await Fetch(GetCachedKeyForStaff(userId), async e =>
        {
            return await _dbContext.Stores
                .Where(s => s.StaffAssignments.Any(a => a.StaffId == userId)).ToListAsync();
        });
        return stores;
    }

    public async Task<List<Store>?> RefreshAndGetCachedStoresByStaffId(int userId)
    {
        var stores = await Refresh(GetCachedKeyForStaff(userId), async e =>
        {
            return await _dbContext.Stores
                .Where(s => s.StaffAssignments.Any(a => a.StaffId == userId)).ToListAsync();
        });
        return stores;
    }

    public void InvalidCachedStoresByStaffId(int userId)
    {
        Invalidate(GetCachedKeyForStaff(userId));
    }

    #endregion

    #region Get stores by tenant
    public async Task<List<Store>?> GetCachedStoresByTenant(int tenantId)
    {
        var stores = await Fetch(GetCachedKeyForTenant(tenantId), async e =>
        {
            return await _dbContext.Stores
                .Where(s => s.TenantId == tenantId).ToListAsync();
        });
        return stores;
    }

    public async Task<List<Store>?> RefreshAndGetCachedStoresByTenant(int tenantId)
    {
        var stores = await Refresh(GetCachedKeyForTenant(tenantId), async e =>
        {
            return await _dbContext.Stores
                .Where(s => s.TenantId == tenantId).ToListAsync();
        });
        return stores;
    }

    public void InvalidCachedStoresByTenant(int tenantId)
    {
        Invalidate(GetCachedKeyForTenant(tenantId));
    }

    #endregion
    private string GetCachedKeyForStaff(int userId)
    {
        return $"{_baseCacheName}stores-staffId-{userId}";
    }
    private string GetCachedKeyForManager(int userId)
    {
        return $"{_baseCacheName}stores-managerId-{userId}";
    }
    private string GetCachedKeyForTenant(int tenantId)
    {
        return $"{_baseCacheName}stores-tenant-{tenantId}";
    }
}

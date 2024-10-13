using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Laundro.Core.Features.UserContextState.Repositories;

public interface IUserStoresRepository
{
    Task<List<Store>?> GetCachedStoresByUser(int userId);
    void InvalidCachedStoresByUser(int userId);
    Task<List<Store>?> RefreshAndGetCachedStoresByUser(int userId);

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

    public async Task<List<Store>?> GetCachedStoresByUser(int userId)
    {
        var stores = await Fetch(GetCachedKeyForUser(userId), async e =>
        {
            return await _dbContext.Stores
                .Include(s => s.StoreUser).ThenInclude(ss => ss.Role)
                .Where(s => s.StoreUser != null && s.StoreUser.Any(ss => ss.UserId == userId))
                .ToListAsync();
        });
        return stores;
    }

    public async Task<List<Store>?> RefreshAndGetCachedStoresByUser(int userId)
    {
        var stores = await Refresh(GetCachedKeyForUser(userId), async e =>
        {
            return await _dbContext.Stores
                .Include(s => s.StoreUser).ThenInclude(ss => ss.Role)
                .Where(s => s.StoreUser != null && s.StoreUser.Any(ss => ss.UserId == userId))
                .ToListAsync();
        });
        return stores;
    }

    public void InvalidCachedStoresByUser(int userId)
    {
        Invalidate(GetCachedKeyForUser(userId));
    }

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

    private string GetCachedKeyForUser(int userId)
    {
        return $"{_baseCacheName}stores-userId-{userId}";
    }
    private string GetCachedKeyForTenant(int tenantId)
    {
        return $"{_baseCacheName}stores-tenant-{tenantId}";
    }
}

using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Laundro.Core.Features.UserContextState.Repositories;

public interface IUserTenantRepository
{
    Task<Tenant?> GetCachedTenantByOwner(int userId);
    Task<Tenant?> RefreshAndGetCachedTenantByOwner(int userId);
    void InvalidateCachedTenantByOwner(int userId);
}

public class UserTenantRepository : BaseCacheService<Tenant>, IUserTenantRepository
{
    private readonly LaundroDbContext _dbContext;

    public UserTenantRepository(ICache cache, LaundroDbContext dbContext) : base(cache)
    {
        _dbContext = dbContext;
    }

    public async Task<Tenant?> GetCachedTenantByOwner(int userId)
    {
        var tenant = await Fetch(GetCachedKey(userId), async e =>
        {
            return await _dbContext.Tenants.FirstOrDefaultAsync(t => t.OwnerId == userId);
        });
        return tenant;
    }

    public async Task<Tenant?> RefreshAndGetCachedTenantByOwner(int userId)
    {
        var tenant = await Refresh(GetCachedKey(userId), async e =>
        {
            return await _dbContext.Tenants.FirstOrDefaultAsync(t => t.OwnerId == userId);
        });
        return tenant;
    }

    public void InvalidateCachedTenantByOwner(int userId)
    {
        Invalidate(GetCachedKey(userId));
    }

    private string GetCachedKey(int userId)
    {
        return $"{_baseCacheName}tenant-by-owner-{userId}";
    }
}

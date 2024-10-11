using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Laundro.Core.Features.ContextState.Repositories;

public interface IUserInfoRepository
{
    Task<User?> GetCachedUserInfo(string userEmail);
    Task<User?> RefreshAndGetCachedUserInfo(string userEmail);
    void InvalidateCachedUserInfo(string userEmail);
}

public class UserInfoRepository : BaseCacheService<User>, IUserInfoRepository
{
    private readonly LaundroDbContext _dbContext;

    public UserInfoRepository(ICache cache, LaundroDbContext dbContext) : base(cache)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetCachedUserInfo(string userEmail)
    {
        var userInfo = await Fetch(GetCachedKey(userEmail), async e =>
        {
            return await _dbContext.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Email == userEmail);
        });
        return userInfo;
    }

    public async Task<User?> RefreshAndGetCachedUserInfo(string userEmail)
    {
        var userInfo = await Refresh(GetCachedKey(userEmail), async e =>
        {
            return await _dbContext.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Email == userEmail);
        });
        return userInfo;
    }

    public void InvalidateCachedUserInfo(string userEmail)
    {
        Invalidate(GetCachedKey(userEmail));
    }

    private string GetCachedKey(string userEmail)
    {
        return $"{_baseCacheName}user-info-{userEmail}";
    }
}

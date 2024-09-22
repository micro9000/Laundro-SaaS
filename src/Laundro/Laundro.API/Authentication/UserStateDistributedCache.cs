using Laundro.Core.Data;
using Microsoft.Extensions.Caching.Distributed;

namespace Laundro.API.Authentication;

public class UserStateDistributedCache : IUserStateCache
{
    private readonly ICache _cache;

    public UserStateDistributedCache(ICache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<UserState> Fetch(string userEmail, Func<DistributedCacheEntryOptions, Task<UserState>> userContextFactory)
    {
        var key = userEmail.ToUpperInvariant();
        return await _cache.GetOrCreateAsync(key, userContextFactory);
    }
    public async Task<UserState> Refresh(string userEmail, Func<DistributedCacheEntryOptions, Task<UserState>> userContextFactory)
    {
        var key = userEmail.ToUpperInvariant();
        await _cache.RemoveAsync(key);
        return await _cache.GetOrCreateAsync(key, userContextFactory);
    }

    public void Invalidate(string userEmail)
    {
        var key = userEmail.ToUpperInvariant();
        _cache.RemoveAsync(key).Wait();
    }

}

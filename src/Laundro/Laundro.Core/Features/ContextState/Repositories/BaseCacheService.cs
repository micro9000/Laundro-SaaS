using Laundro.Core.Data;
using Microsoft.Extensions.Caching.Distributed;

namespace Laundro.Core.Features.ContextState.Repositories;
public abstract class BaseCacheService<T>
{
    private readonly ICache _cache;
    public readonly string _baseCacheName = "UserAccountState:";

    public BaseCacheService(ICache cache)
    {
        _cache = cache;
    }

    public async Task<T?> Fetch(string cacheKey, Func<DistributedCacheEntryOptions, Task<T?>> userContextFactory)
    {
        var key = cacheKey.ToUpperInvariant();
        return await _cache.GetOrCreateAsync(key, userContextFactory);
    }

    public async Task<T?> Refresh(string cacheKey, Func<DistributedCacheEntryOptions, Task<T?>> userContextFactory)
    {
        var key = cacheKey.ToUpperInvariant();
        await _cache.RemoveAsync(key);
        return await _cache.GetOrCreateAsync(key, userContextFactory);
    }

    public void Invalidate(string cacheKey)
    {
        var key = cacheKey.ToUpperInvariant();
        _cache.RemoveAsync(key).Wait();
    }
}

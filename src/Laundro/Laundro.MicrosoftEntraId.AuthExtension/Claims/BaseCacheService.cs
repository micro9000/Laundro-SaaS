using Laundro.MicrosoftEntraId.AuthExtension.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;

namespace Laundro.MicrosoftEntraId.AuthExtension.Claims;
public abstract class BaseCacheService<T>
{
    private readonly ICache _cache;

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

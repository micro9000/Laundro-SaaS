using Laundro.MicrosoftEntraId.AuthExtension.Caching;
using System;
using System.Threading.Tasks;

namespace Laundro.MicrosoftEntraId.AuthExtension.Data;
public abstract class BaseCacheService<T>
{
    private readonly ICache _cache;

    public BaseCacheService(ICache cache)
    {
        _cache = cache;
    }

    public async Task<T?> Fetch(string cacheKey, Func<Task<T?>> userContextFactory, TimeSpan? expirationFromNow = null)
    {
        var key = cacheKey.ToUpperInvariant();
        return await _cache.GetOrCreateAsync(key, userContextFactory, expirationFromNow);
    }

    public async Task<T?> Refresh(string cacheKey, Func<Task<T?>> userContextFactory, TimeSpan? expirationFromNow = null)
    {
        var key = cacheKey.ToUpperInvariant();
        await _cache.RemoveAsync(key);
        return await _cache.GetOrCreateAsync(key, userContextFactory, expirationFromNow);
    }

    public void Invalidate(string cacheKey)
    {
        var key = cacheKey.ToUpperInvariant();
        _cache.RemoveAsync(key).Wait();
    }
}

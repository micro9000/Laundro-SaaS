using Microsoft.Extensions.Caching.Distributed;

namespace Laundro.Core.Data;
public interface ICache
{
    Task<TData> GetOrCreateAsync<TData>(
        string cacheKey,
        Func<DistributedCacheEntryOptions, Task<TData>> valueGetter,
        DistributedCacheEntryOptions? options = null);

    Task RemoveAsync(string key);
}

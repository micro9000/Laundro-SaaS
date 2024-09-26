using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;

namespace Laundro.MicrosoftEntraId.AuthExtension.Caching;
public interface ICache
{
    Task<TData> GetOrCreateAsync<TData>(
        string cacheKey,
        Func<DistributedCacheEntryOptions, Task<TData>> valueGetter,
        DistributedCacheEntryOptions? options = null);

    Task RemoveAsync(string key);
}

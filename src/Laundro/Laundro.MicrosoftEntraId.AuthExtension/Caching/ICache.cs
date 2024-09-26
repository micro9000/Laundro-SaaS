using System;
using System.Threading.Tasks;

namespace Laundro.MicrosoftEntraId.AuthExtension.Caching;
public interface ICache
{
    Task<TData?> GetOrCreateAsync<TData>(
        string cacheKey,
        Func<Task<TData>> valueGetter,
        TimeSpan? options = null);

    Task RemoveAsync(string key);
}

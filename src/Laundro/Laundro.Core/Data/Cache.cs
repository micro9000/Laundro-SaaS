using Microsoft.Extensions.Caching.Distributed;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Laundro.Core.Data;
public class Cache : ICache
{
    private readonly IDistributedCache _cache;

    public Cache(IDistributedCache cache)
    {
        _cache = cache;
    }
#if DEBUG
    private static TimeSpan DefaultAbsoluteExpirationFromNow => TimeSpan.FromSeconds(30);
# else
    private static TimeSpan DefaultAbsoluteExpirationFromNow => TimeSpan.FromMinutes(5);
# endif

    private static JsonSerializerOptions CacheSerializerOptions { get; } = new()
    {
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        MaxDepth = 32,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        IgnoreReadOnlyProperties = false,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        IgnoreReadOnlyFields = true,
        NumberHandling = JsonNumberHandling.Strict,
        IncludeFields = true
    };

    public async Task<TData> GetOrCreateAsync<TData>(
        string cacheKey, 
        Func<DistributedCacheEntryOptions, Task<TData>> valueGetter, 
        DistributedCacheEntryOptions? options = null)
    {
        var (val, exists) = await GetAsync<TData>(cacheKey);

        if (exists)
        {
            return val;
        }

        return await LoadAsync(cacheKey, valueGetter,
            options ?? new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = DefaultAbsoluteExpirationFromNow,
            });

    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }

    private async Task<(TData? value, bool exists)> GetAsync<TData>(string cacheKey)
    {
        var bytes = await _cache.GetAsync(cacheKey);
        if (bytes is not { Length: > 0 })
        {
            return (default, false);
        }    

        var data = JsonSerializer.Deserialize<TData>(Encoding.UTF8.GetString(bytes), CacheSerializerOptions);
        if (data is null)
        {
            throw new SerializationException($"Failed to deserialize cache item with key: {cacheKey}");
        }
        return (data, true);
    }

    private async Task<TData> LoadAsync<TData>(
        string cacheKey,
        Func<DistributedCacheEntryOptions, Task<TData>> valueGetter,
        DistributedCacheEntryOptions options)
    {
        var cacheEntry = await valueGetter(options);
        if (cacheEntry is not null)
        {
            var val = JsonSerializer.SerializeToUtf8Bytes(cacheEntry, CacheSerializerOptions);
            await _cache.SetAsync(cacheKey, val, options);
        }
        return cacheEntry;
    }

}

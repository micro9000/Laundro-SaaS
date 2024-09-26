using StackExchange.Redis;
using System;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Laundro.MicrosoftEntraId.AuthExtension.Caching;
public class Cache : ICache
{
    private readonly IDatabase _redisDb;

    public Cache(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    private static TimeSpan DefaultAbsoluteExpirationFromNow => TimeSpan.FromHours(1);

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

    public async Task<TData?> GetOrCreateAsync<TData>(
        string cacheKey, 
        Func<Task<TData>> valueGetter, 
        TimeSpan? expirationFromNow = null)
    {
        var (val, exists) = await GetAsync<TData>(cacheKey);

        if (exists)
        {
            return val;
        }

        return await LoadAsync(cacheKey, valueGetter,
            expirationFromNow ?? DefaultAbsoluteExpirationFromNow);
    }
    public async Task RemoveAsync(string key)
    {
        await _redisDb.StringGetDeleteAsync(key);
    }

    private async Task<(TData? value, bool exists)> GetAsync<TData>(string cacheKey)
    {
        var bytes = await _redisDb.StringGetAsync(cacheKey);
        if (bytes != RedisValue.Null)
        {
            return (default, false);
        }    

        var data = JsonSerializer.Deserialize<TData>(bytes, CacheSerializerOptions);
        if (data is null)
        {
            throw new SerializationException($"Failed to deserialize cache item with key: {cacheKey}");
        }
        return (data, true);
    }

    private async Task<TData?> LoadAsync<TData>(
        string cacheKey,
        Func<Task<TData>> valueGetter,
        TimeSpan expirationFromNow)
    {
        var cacheEntry = await valueGetter();
        if (cacheEntry is not null)
        {
            var val = JsonSerializer.Serialize(cacheEntry, CacheSerializerOptions);
            await _redisDb.StringSetAsync(cacheKey, val, expirationFromNow);
        }
        return cacheEntry;
    }

}

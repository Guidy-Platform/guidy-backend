
using CoursePlatform.Application.Contracts.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace CoursePlatform.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    // IDatabase from StackExchange.Redis to interact with Redis and perform cache operations
    private readonly IDatabase _db;
    // IConnectionMultiplexer  to manage the connection to Redis
    public RedisCacheService(IConnectionMultiplexer redis)
        => _db = redis.GetDatabase();

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var value = await _db.StringGetAsync(key);
        return value.IsNullOrEmpty
            ? default
            : JsonSerializer.Deserialize<T>(value!);
    }

    public async Task SetAsync<T>(
        string key, T value,
        TimeSpan? expiry = null, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, json, expiry ?? TimeSpan.FromMinutes(10));
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
        => await _db.KeyDeleteAsync(key);

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        // في production استخدم Lua script بدل ده
        var server = _db.Multiplexer.GetServer(
            _db.Multiplexer.GetEndPoints().First());
        var keys = server.Keys(pattern: $"{prefix}*").ToArray();
        if (keys.Length > 0)
            await _db.KeyDeleteAsync(keys);
    }
}
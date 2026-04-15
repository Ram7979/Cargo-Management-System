using System.Text.Json;
using CMS.FleetService.Application.Interfaces;
using StackExchange.Redis;

namespace CMS.FleetService.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(key);
        if (!value.HasValue)
            return default;

        return JsonSerializer.Deserialize<T>((string)value!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var serialized = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, serialized, expiry);
    }

    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    public async Task<IEnumerable<T>> GetByPatternAsync<T>(string pattern)
    {
        var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
        var keys = server.Keys(pattern: pattern).ToArray();

        var results = new List<T>();
        foreach (var key in keys)
        {
            var value = await _database.StringGetAsync(key);
            if (value.HasValue)
            {
                var item = JsonSerializer.Deserialize<T>((string)value!);
                if (item != null)
                    results.Add(item);
            }
        }

        return results;
    }
}

using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace API.Shared.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
  private readonly IDistributedCache _cache;
  private readonly IConnectionMultiplexer _redis;
  private readonly string _instanceName;

  public RedisCacheService(
      IDistributedCache cache,
      IConnectionMultiplexer redis,
      string instanceName)
  {
    _cache = cache;
    _redis = redis;
    _instanceName = instanceName;
  }

  public async Task<T?> GetAsync<T>(string key)
  {
    var value = await _cache.GetStringAsync(GetFullKey(key));
    return value == null ? default : JsonSerializer.Deserialize<T>(value);
  }

  public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
  {
    var options = new DistributedCacheEntryOptions
    {
      AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(1)
    };

    await _cache.SetStringAsync(
        GetFullKey(key),
        JsonSerializer.Serialize(value),
        options);
  }

  public Task RemoveAsync(string key)
  {
    return _cache.RemoveAsync(GetFullKey(key));
  }

  public async Task RemoveByPrefixAsync(string prefix)
  {
    prefix = GetFullKey(prefix);
    var endpoints = _redis.GetEndPoints();
    var server = _redis.GetServer(endpoints.First());
    var keys = server.Keys(pattern: $"{prefix}*");

    foreach (var key in keys)
    {
      await _cache.RemoveAsync(key.ToString());
    }
  }

  public async Task<bool> ExistsAsync(string key)
  {
    return await _cache.GetAsync(GetFullKey(key)) != null;
  }

  public async Task<IDistributedLockHandle?> CreateLockAsync(string key, TimeSpan expirationTime)
  {
    var lockKey = $"lock:{GetFullKey(key)}";
    var db = _redis.GetDatabase();
    var lockValue = Guid.NewGuid().ToString();

    var acquired = await db.StringSetAsync(
        lockKey,
        lockValue,
        expirationTime,
        When.NotExists);

    if (!acquired)
    {
      return null;
    }

    return new RedisLockHandle(db, lockKey, lockValue);
  }

  private string GetFullKey(string key) => $"{_instanceName}{key}";
}

internal class RedisLockHandle : IDistributedLockHandle
{
  private readonly IDatabase _database;
  private readonly string _lockKey;
  private readonly string _lockValue;
  private bool _isDisposed;

  public RedisLockHandle(IDatabase database, string lockKey, string lockValue)
  {
    _database = database;
    _lockKey = lockKey;
    _lockValue = lockValue;
    IsAcquired = true;
  }

  public string LockKey => _lockKey;
  public bool IsAcquired { get; private set; }

  public async Task ReleaseAsync()
  {
    if (!_isDisposed && IsAcquired)
    {
      var script = @"
                if redis.call('get', KEYS[1]) == ARGV[1] then
                    return redis.call('del', KEYS[1])
                else
                    return 0
                end";

      await _database.ScriptEvaluateAsync(
          script,
          new RedisKey[] { _lockKey },
          new RedisValue[] { _lockValue });

      IsAcquired = false;
    }
  }

  public async ValueTask DisposeAsync()
  {
    if (!_isDisposed)
    {
      await ReleaseAsync();
      _isDisposed = true;
    }
  }
}
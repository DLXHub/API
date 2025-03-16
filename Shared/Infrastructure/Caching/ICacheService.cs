namespace API.Shared.Infrastructure.Caching;

public interface ICacheService
{
  Task<T?> GetAsync<T>(string key);
  Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
  Task RemoveAsync(string key);
  Task RemoveByPrefixAsync(string prefix);
  Task<bool> ExistsAsync(string key);
  Task<IDistributedLockHandle?> CreateLockAsync(string key, TimeSpan expirationTime);
}
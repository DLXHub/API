using API.Features.FeatureFlags.Models;
using API.Shared.Infrastructure;
using API.Shared.Infrastructure.Caching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Features.FeatureFlags.Services;

public interface IFeatureFlagService
{
  Task<bool> IsEnabledAsync(string key);
  Task<T?> GetConfigurationValueAsync<T>(string key, string configKey);
  Task<Dictionary<string, bool>> GetClientFlagsAsync();
  Task<IEnumerable<FeatureFlag>> GetAllFlagsAsync();
  Task<FeatureFlag?> GetFlagAsync(string key);
  Task<FeatureFlag> CreateFlagAsync(FeatureFlag flag);
  Task<FeatureFlag?> UpdateFlagAsync(string key, Action<FeatureFlag> updateAction);
  Task<bool> DeleteFlagAsync(string key);
}

public class FeatureFlagService : IFeatureFlagService
{
  private readonly ApplicationDbContext _context;
  private readonly ICacheService _cache;
  private readonly ILogger<FeatureFlagService> _logger;
  private const string CacheKeyPrefix = "feature-flag:";
  private const string ClientFlagsCacheKey = "feature-flags:client";

  public FeatureFlagService(
      ApplicationDbContext context,
      ICacheService cache,
      ILogger<FeatureFlagService> logger)
  {
    _context = context;
    _cache = cache;
    _logger = logger;
  }

  public async Task<bool> IsEnabledAsync(string key)
  {
    var cacheKey = $"{CacheKeyPrefix}{key}";
    var cached = await _cache.GetAsync<bool?>(cacheKey);

    if (cached.HasValue)
      return cached.Value;

    var flag = await _context.FeatureFlags
        .AsNoTracking()
        .FirstOrDefaultAsync(f => f.Key == key);

    var isEnabled = flag?.IsEnabled ?? false;
    await _cache.SetAsync(cacheKey, isEnabled, TimeSpan.FromMinutes(5));

    return isEnabled;
  }

  public async Task<T?> GetConfigurationValueAsync<T>(string key, string configKey)
  {
    var flag = await GetFlagAsync(key);
    if (flag?.Configuration == null || !flag.Configuration.ContainsKey(configKey))
      return default;

    return (T)flag.Configuration[configKey];
  }

  public async Task<Dictionary<string, bool>> GetClientFlagsAsync()
  {
    var cached = await _cache.GetAsync<Dictionary<string, bool>>(ClientFlagsCacheKey);
    if (cached != null)
      return cached;

    var flags = await _context.FeatureFlags
        .AsNoTracking()
        .Where(f => f.ClientKey != null)
        .ToDictionaryAsync(f => f.ClientKey!, f => f.IsEnabled);

    await _cache.SetAsync(ClientFlagsCacheKey, flags, TimeSpan.FromMinutes(5));

    return flags;
  }

  public async Task<IEnumerable<FeatureFlag>> GetAllFlagsAsync()
  {
    return await _context.FeatureFlags
        .AsNoTracking()
        .OrderBy(f => f.Category)
        .ThenBy(f => f.Key)
        .ToListAsync();
  }

  public async Task<FeatureFlag?> GetFlagAsync(string key)
  {
    return await _context.FeatureFlags
        .AsNoTracking()
        .FirstOrDefaultAsync(f => f.Key == key);
  }

  public async Task<FeatureFlag> CreateFlagAsync(FeatureFlag flag)
  {
    _context.FeatureFlags.Add(flag);
    await _context.SaveChangesAsync();
    await InvalidateCacheAsync(flag.Key);
    return flag;
  }

  public async Task<FeatureFlag?> UpdateFlagAsync(string key, Action<FeatureFlag> updateAction)
  {
    var flag = await _context.FeatureFlags.FirstOrDefaultAsync(f => f.Key == key);
    if (flag == null)
      return null;

    updateAction(flag);
    await _context.SaveChangesAsync();
    await InvalidateCacheAsync(key);

    return flag;
  }

  public async Task<bool> DeleteFlagAsync(string key)
  {
    var flag = await _context.FeatureFlags.FirstOrDefaultAsync(f => f.Key == key);
    if (flag == null)
      return false;

    _context.FeatureFlags.Remove(flag);
    await _context.SaveChangesAsync();
    await InvalidateCacheAsync(key);

    return true;
  }

  private async Task InvalidateCacheAsync(string key)
  {
    await _cache.RemoveAsync($"{CacheKeyPrefix}{key}");
    await _cache.RemoveAsync(ClientFlagsCacheKey);
  }
}
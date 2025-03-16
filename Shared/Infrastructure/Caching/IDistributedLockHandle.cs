namespace API.Shared.Infrastructure.Caching;

public interface IDistributedLockHandle : IAsyncDisposable
{
  string LockKey { get; }
  bool IsAcquired { get; }
  Task ReleaseAsync();
}
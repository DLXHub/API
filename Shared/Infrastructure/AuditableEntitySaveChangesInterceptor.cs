using API.Shared.Extensions;
using API.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace API.Shared.Infrastructure;

public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public AuditableEntitySaveChangesInterceptor(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
  {
    UpdateEntities(eventData.Context);
    return base.SavingChanges(eventData, result);
  }

  public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
  {
    UpdateEntities(eventData.Context);
    return base.SavingChangesAsync(eventData, result, cancellationToken);
  }

  private void UpdateEntities(DbContext? context)
  {
    if (context == null) return;

    var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

    foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
    {
      if (entry.State == EntityState.Added)
      {
        entry.Entity.CreatedById = userId;
        entry.Entity.CreatedOn = DateTime.UtcNow;
      }

      if (entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
      {
        entry.Entity.ModifiedById = userId;
        entry.Entity.ModifiedOn = DateTime.UtcNow;
      }

      if (entry.State == EntityState.Deleted)
      {
        entry.State = EntityState.Modified;
        entry.Entity.DeletedById = userId;
        entry.Entity.DeletedOn = DateTime.UtcNow;
        entry.Entity.IsDeleted = true;
      }
    }
  }
}
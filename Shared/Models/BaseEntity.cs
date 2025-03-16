using Microsoft.AspNetCore.Identity;

namespace API.Shared.Models;

public abstract class BaseEntity
{
  public Guid Id { get; set; }

  public DateTime CreatedOn { get; set; }
  public string? CreatedById { get; set; }
  public virtual ApplicationUser? CreatedBy { get; set; }

  public DateTime? ModifiedOn { get; set; }
  public string? ModifiedById { get; set; }
  public virtual ApplicationUser? ModifiedBy { get; set; }

  public DateTime? DeletedOn { get; set; }
  public string? DeletedById { get; set; }
  public virtual ApplicationUser? DeletedBy { get; set; }
  public bool IsDeleted { get; set; }

  protected BaseEntity()
  {
    Id = Guid.NewGuid();
    CreatedOn = DateTime.UtcNow;
  }
}
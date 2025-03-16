using System.ComponentModel.DataAnnotations;
using API.Shared.Models;

namespace API.Features.Languages.Models;

public class Language : BaseEntity
{
  [Required]
  [StringLength(2, MinimumLength = 2)]
  public string IsoCode { get; set; } = string.Empty;

  [Required]
  [StringLength(50)]
  public string Name { get; set; } = string.Empty;

  public bool IsActive { get; set; } = true;
  public bool IsDefault { get; set; }

  [StringLength(50)]
  public string? FlagIcon { get; set; }

  // Navigation property for translations
  public virtual ICollection<MediaTranslation> MediaTranslations { get; set; }
      = new List<MediaTranslation>();
}
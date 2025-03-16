using System.ComponentModel.DataAnnotations;

namespace API.Features.Languages.Models.Dtos;

public class UpdateLanguageDto
{
  [Required]
  [StringLength(50)]
  public string Name { get; set; } = string.Empty;

  public bool IsActive { get; set; }

  [StringLength(50)]
  public string? FlagIcon { get; set; }
}
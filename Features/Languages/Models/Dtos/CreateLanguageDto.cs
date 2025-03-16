using System.ComponentModel.DataAnnotations;

namespace API.Features.Languages.Models.Dtos;

public class CreateLanguageDto
{
  [Required]
  [StringLength(2, MinimumLength = 2)]
  public string IsoCode { get; set; } = string.Empty;

  [Required]
  [StringLength(50)]
  public string Name { get; set; } = string.Empty;

  [StringLength(50)]
  public string? FlagIcon { get; set; }
}
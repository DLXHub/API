using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using API.Features.Movies.Models;
using API.Features.TvShows.Models;
using API.Shared.Models;

namespace API.Features.Languages.Models;

public class MediaTranslation : BaseEntity
{
  [Required]
  public Guid MediaId { get; set; }

  [Required]
  [StringLength(2, MinimumLength = 2)]
  public string LanguageCode { get; set; } = string.Empty;

  [Required]
  [StringLength(255)]
  public string Title { get; set; } = string.Empty;

  public string? Overview { get; set; }

  [Required]
  [StringLength(255)]
  public string Slug { get; set; } = string.Empty;

  // Navigation properties
  public virtual Movie? Movie { get; set; }
  public virtual TvShow? TvShow { get; set; }
  public virtual Language Language { get; set; } = null!;

  // Additional metadata as JSON
  public JsonDocument? MetaData { get; set; }
}
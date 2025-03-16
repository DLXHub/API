using System.ComponentModel.DataAnnotations;
using API.Features.Genres.Models;

namespace API.Shared.Models;

/// <summary>
/// Join entity for the many-to-many relationship between media entities and genres.
/// </summary>
public class MediaGenre
{
  /// <summary>
  /// The ID of the media entity (Movie or TvShow).
  /// </summary>
  [Key]
  public Guid MediaId { get; set; }

  /// <summary>
  /// The media entity (Movie or TvShow).
  /// </summary>
  public virtual MediaEntity Media { get; set; } = null!;

  /// <summary>
  /// The ID of the genre.
  /// </summary>
  public Guid GenreId { get; set; }

  /// <summary>
  /// The genre.
  /// </summary>
  public virtual Genre Genre { get; set; } = null!;
}
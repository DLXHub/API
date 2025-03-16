using API.Features.Movies.Models;
using API.Features.TvShows.Models;
using API.Shared.Models;

namespace API.Features.Genres.Models;

/// <summary>
/// Represents a genre in the system.
/// </summary>
public class Genre : BaseEntity
{
  /// <summary>
  /// The TMDB ID of the genre.
  /// </summary>
  public int TmdbId { get; set; }

  /// <summary>
  /// The name of the genre.
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// The movies that belong to this genre.
  /// </summary>
  public virtual ICollection<MediaGenre> MediaGenres { get; set; } = new List<MediaGenre>();
}
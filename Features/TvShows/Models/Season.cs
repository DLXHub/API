using API.Shared.Models;

namespace API.Features.TvShows.Models;

/// <summary>
/// Represents a season of a TV show.
/// </summary>
public class Season : BaseEntity
{
  /// <summary>
  /// The TMDB ID of the season.
  /// </summary>
  public int TmdbId { get; set; }

  /// <summary>
  /// The TV show this season belongs to.
  /// </summary>
  public Guid TvShowId { get; set; }

  /// <summary>
  /// The TV show this season belongs to.
  /// </summary>
  public virtual TvShow TvShow { get; set; } = null!;

  /// <summary>
  /// The season number.
  /// </summary>
  public int SeasonNumber { get; set; }

  /// <summary>
  /// The name of the season.
  /// </summary>
  public string? Name { get; set; }

  /// <summary>
  /// The overview or description of the season.
  /// </summary>
  public string? Overview { get; set; }

  /// <summary>
  /// The poster path for the season.
  /// </summary>
  public string? PosterPath { get; set; }

  /// <summary>
  /// The air date of the season.
  /// </summary>
  public DateTime? AirDate { get; set; }

  /// <summary>
  /// The number of episodes in the season.
  /// </summary>
  public int? EpisodeCount { get; set; }

  /// <summary>
  /// The episodes in this season.
  /// </summary>
  public virtual ICollection<Episode> Episodes { get; set; } = new List<Episode>();
}
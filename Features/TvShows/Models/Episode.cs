using API.Shared.Models;

namespace API.Features.TvShows.Models;

/// <summary>
/// Represents an episode of a TV show season.
/// </summary>
public class Episode : BaseEntity
{
  /// <summary>
  /// The TMDB ID of the episode.
  /// </summary>
  public int TmdbId { get; set; }

  /// <summary>
  /// The season this episode belongs to.
  /// </summary>
  public Guid SeasonId { get; set; }

  /// <summary>
  /// The season this episode belongs to.
  /// </summary>
  public virtual Season Season { get; set; } = null!;

  /// <summary>
  /// The episode number within the season.
  /// </summary>
  public int EpisodeNumber { get; set; }

  /// <summary>
  /// The name of the episode.
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// The overview or description of the episode.
  /// </summary>
  public string? Overview { get; set; }

  /// <summary>
  /// The still image path for the episode.
  /// </summary>
  public string? StillPath { get; set; }

  /// <summary>
  /// The air date of the episode.
  /// </summary>
  public DateTime? AirDate { get; set; }

  /// <summary>
  /// The runtime of the episode in minutes.
  /// </summary>
  public int? Runtime { get; set; }

  /// <summary>
  /// The average vote for the episode.
  /// </summary>
  public decimal? VoteAverage { get; set; }

  /// <summary>
  /// The number of votes for the episode.
  /// </summary>
  public int? VoteCount { get; set; }

  /// <summary>
  /// The crew members for the episode (stored as a JSON string).
  /// </summary>
  public string? Crew { get; set; }

  /// <summary>
  /// The guest stars for the episode (stored as a JSON string).
  /// </summary>
  public string? GuestStars { get; set; }

  /// <summary>
  /// The production code of the episode.
  /// </summary>
  public string? ProductionCode { get; set; }
}
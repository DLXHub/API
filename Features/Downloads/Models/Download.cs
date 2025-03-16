using API.Features.Movies.Models;
using API.Features.TvShows.Models;
using API.Shared.Models;

namespace API.Features.Downloads.Models;

/// <summary>
/// Represents a download in the system.
/// </summary>
public class Download : BaseEntity
{
  /// <summary>
  /// The title of the download.
  /// </summary>
  public string Title { get; set; } = string.Empty;

  /// <summary>
  /// The language of the download (e.g., "English", "German").
  /// </summary>
  public string Language { get; set; } = string.Empty;

  /// <summary>
  /// The quality of the download (e.g., "1080p", "4K", "HDR").
  /// </summary>
  public string Quality { get; set; } = string.Empty;

  /// <summary>
  /// The type of media this download is associated with.
  /// </summary>
  public MediaType MediaType { get; set; }

  /// <summary>
  /// The ID of the movie, if this download is associated with a movie.
  /// </summary>
  public Guid? MovieId { get; set; }

  /// <summary>
  /// The movie this download is associated with, if any.
  /// </summary>
  public virtual Movie? Movie { get; set; }

  /// <summary>
  /// The ID of the TV show, if this download is associated with a TV show.
  /// </summary>
  public Guid? TvShowId { get; set; }

  /// <summary>
  /// The TV show this download is associated with, if any.
  /// </summary>
  public virtual TvShow? TvShow { get; set; }

  /// <summary>
  /// The ID of the season, if this download is associated with a season.
  /// </summary>
  public Guid? SeasonId { get; set; }

  /// <summary>
  /// The season this download is associated with, if any.
  /// </summary>
  public virtual Season? Season { get; set; }

  /// <summary>
  /// The ID of the episode, if this download is associated with an episode.
  /// </summary>
  public Guid? EpisodeId { get; set; }

  /// <summary>
  /// The episode this download is associated with, if any.
  /// </summary>
  public virtual Episode? Episode { get; set; }
}

/// <summary>
/// Represents the type of media a download can be associated with.
/// </summary>
public enum MediaType
{
  /// <summary>
  /// The download is associated with a movie.
  /// </summary>
  Movie,

  /// <summary>
  /// The download is associated with a TV show.
  /// </summary>
  TvShow,

  /// <summary>
  /// The download is associated with a season.
  /// </summary>
  Season,

  /// <summary>
  /// The download is associated with an episode.
  /// </summary>
  Episode
}
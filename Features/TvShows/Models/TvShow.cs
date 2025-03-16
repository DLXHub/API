using API.Features.People.Models;
using API.Shared.Models;

namespace API.Features.TvShows.Models;

/// <summary>
/// Represents a TV show in the system.
/// </summary>
public class TvShow : MediaEntity
{
  /// <summary>
  /// The name of the TV show.
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// The original name of the TV show.
  /// </summary>
  public string? OriginalName { get; set; }

  /// <summary>
  /// The first air date of the TV show.
  /// </summary>
  public DateTime? FirstAirDate { get; set; }

  /// <summary>
  /// The last air date of the TV show.
  /// </summary>
  public DateTime? LastAirDate { get; set; }

  /// <summary>
  /// The status of the TV show (e.g., "Returning Series", "Ended", etc.).
  /// </summary>
  public string? Status { get; set; }

  /// <summary>
  /// The type of the TV show (e.g., "Scripted", "Reality", etc.).
  /// </summary>
  public string? Type { get; set; }

  /// <summary>
  /// The number of seasons in the TV show.
  /// </summary>
  public int? NumberOfSeasons { get; set; }

  /// <summary>
  /// The number of episodes in the TV show.
  /// </summary>
  public int? NumberOfEpisodes { get; set; }

  /// <summary>
  /// The average episode runtime in minutes.
  /// </summary>
  public int? EpisodeRunTime { get; set; }

  /// <summary>
  /// The networks that broadcast the TV show.
  /// </summary>
  public string? Networks { get; set; }

  /// <summary>
  /// Indicates whether the TV show is in production.
  /// </summary>
  public bool? InProduction { get; set; }

  /// <summary>
  /// The genres of the TV show (stored as a comma-separated string).
  /// </summary>
  public string? GenresString { get; set; }

  /// <summary>
  /// The origin country codes of the TV show (stored as a comma-separated string).
  /// </summary>
  public string? OriginCountry { get; set; }

  /// <summary>
  /// The original language of the TV show.
  /// </summary>
  public string? OriginalLanguage { get; set; }

  /// <summary>
  /// The date when the TV show data was last updated.
  /// </summary>
  public DateTime? LastUpdated { get; set; }

  /// <summary>
  /// The seasons of the TV show.
  /// </summary>
  public virtual ICollection<Season> Seasons { get; set; } = new List<Season>();

  /// <summary>
  /// The cast members of the TV show.
  /// </summary>
  public virtual ICollection<TvShowCast> Cast { get; set; } = new List<TvShowCast>();

  /// <summary>
  /// The crew members of the TV show.
  /// </summary>
  public virtual ICollection<TvShowCrew> Crew { get; set; } = new List<TvShowCrew>();
}
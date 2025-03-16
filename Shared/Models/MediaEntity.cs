using System.ComponentModel.DataAnnotations.Schema;
using API.Features.Genres.Models;

namespace API.Shared.Models;

/// <summary>
/// Base class for media entities like movies and TV shows.
/// This class is not mapped to a database table and only serves as a base for inheritance.
/// </summary>
[NotMapped]
public abstract class MediaEntity : BaseEntity
{
  /// <summary>
  /// The TMDB ID of the media.
  /// </summary>
  public int TmdbId { get; set; }

  /// <summary>
  /// The URL-friendly slug for the media.
  /// </summary>
  public string? Slug { get; set; }

  /// <summary>
  /// The overview or description of the media.
  /// </summary>
  public string? Overview { get; set; }

  /// <summary>
  /// The poster path for the media.
  /// </summary>
  public string? PosterPath { get; set; }

  /// <summary>
  /// The backdrop path for the media.
  /// </summary>
  public string? BackdropPath { get; set; }

  /// <summary>
  /// The average vote for the media.
  /// </summary>
  public decimal? VoteAverage { get; set; }

  /// <summary>
  /// The number of votes for the media.
  /// </summary>
  public int? VoteCount { get; set; }

  /// <summary>
  /// The popularity score of the media.
  /// </summary>
  public decimal? Popularity { get; set; }

  /// <summary>
  /// The genres associated with this media.
  /// </summary>
  public virtual ICollection<MediaGenre> Genres { get; set; } = new List<MediaGenre>();
}
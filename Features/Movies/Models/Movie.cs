using API.Features.Collections.Models;
using API.Features.People.Models;
using API.Shared.Models;

namespace API.Features.Movies.Models;

/// <summary>
/// Represents a movie in the system.
/// </summary>
public class Movie : MediaEntity
{
  /// <summary>
  /// The title of the movie.
  /// </summary>
  public string Title { get; set; } = string.Empty;

  /// <summary>
  /// The original title of the movie.
  /// </summary>
  public string? OriginalTitle { get; set; }

  /// <summary>
  /// The release date of the movie.
  /// </summary>
  public DateTime? ReleaseDate { get; set; }

  /// <summary>
  /// The runtime of the movie in minutes.
  /// </summary>
  public int? Runtime { get; set; }

  /// <summary>
  /// The collections this movie belongs to.
  /// </summary>
  public virtual ICollection<MovieCollection> MovieCollections { get; set; } = new List<MovieCollection>();

  /// <summary>
  /// The cast members of the movie.
  /// </summary>
  public virtual ICollection<MovieCast> Cast { get; set; } = new List<MovieCast>();

  /// <summary>
  /// The crew members of the movie.
  /// </summary>
  public virtual ICollection<MovieCrew> Crew { get; set; } = new List<MovieCrew>();
}
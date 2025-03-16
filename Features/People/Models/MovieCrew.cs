using API.Features.Movies.Models;
using API.Shared.Models;

namespace API.Features.People.Models;

/// <summary>
/// Represents a crew member (director, writer, etc.) in a movie.
/// </summary>
public class MovieCrew : BaseEntity
{
  /// <summary>
  /// The ID of the movie.
  /// </summary>
  public Guid MovieId { get; set; }

  /// <summary>
  /// The movie.
  /// </summary>
  public virtual Movie Movie { get; set; } = null!;

  /// <summary>
  /// The ID of the person.
  /// </summary>
  public Guid PersonId { get; set; }

  /// <summary>
  /// The person (crew member).
  /// </summary>
  public virtual Person Person { get; set; } = null!;

  /// <summary>
  /// The department the crew member worked in (e.g., "Directing", "Writing").
  /// </summary>
  public string? Department { get; set; }

  /// <summary>
  /// The job of the crew member (e.g., "Director", "Screenplay").
  /// </summary>
  public string? Job { get; set; }

  /// <summary>
  /// The TMDB credit ID.
  /// </summary>
  public string? CreditId { get; set; }
}
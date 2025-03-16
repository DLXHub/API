using API.Features.Movies.Models;
using API.Shared.Models;

namespace API.Features.People.Models;

/// <summary>
/// Represents a cast member (actor) in a movie.
/// </summary>
public class MovieCast : BaseEntity
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
  /// The person (actor).
  /// </summary>
  public virtual Person Person { get; set; } = null!;

  /// <summary>
  /// The character name played by the actor.
  /// </summary>
  public string? Character { get; set; }

  /// <summary>
  /// The order of the cast member in the credits.
  /// </summary>
  public int Order { get; set; }

  /// <summary>
  /// The TMDB credit ID.
  /// </summary>
  public string? CreditId { get; set; }
}
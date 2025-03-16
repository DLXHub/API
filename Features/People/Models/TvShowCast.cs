using API.Features.TvShows.Models;
using API.Shared.Models;

namespace API.Features.People.Models;

/// <summary>
/// Represents a cast member (actor) in a TV show.
/// </summary>
public class TvShowCast : BaseEntity
{
  /// <summary>
  /// The ID of the TV show.
  /// </summary>
  public Guid TvShowId { get; set; }

  /// <summary>
  /// The TV show.
  /// </summary>
  public virtual TvShow TvShow { get; set; } = null!;

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
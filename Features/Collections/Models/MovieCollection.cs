using API.Features.Movies.Models;
using API.Shared.Models;

namespace API.Features.Collections.Models;

/// <summary>
/// Represents the many-to-many relationship between movies and collections.
/// </summary>
public class MovieCollection : BaseEntity
{
  /// <summary>
  /// The ID of the movie.
  /// </summary>
  public Guid MovieId { get; set; }

  /// <summary>
  /// The movie associated with this relationship.
  /// </summary>
  public virtual Movie Movie { get; set; } = null!;

  /// <summary>
  /// The ID of the collection.
  /// </summary>
  public Guid CollectionId { get; set; }

  /// <summary>
  /// The collection associated with this relationship.
  /// </summary>
  public virtual Collection Collection { get; set; } = null!;

  /// <summary>
  /// The order of the movie in the collection.
  /// </summary>
  public int Order { get; set; }

  /// <summary>
  /// Notes about the movie in this collection.
  /// </summary>
  public string? Notes { get; set; }

  /// <summary>
  /// The date when the movie was added to the collection.
  /// </summary>
  public DateTime AddedOn { get; set; } = DateTime.UtcNow;
}
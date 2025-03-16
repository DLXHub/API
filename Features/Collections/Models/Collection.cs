using API.Shared.Models;

namespace API.Features.Collections.Models;

/// <summary>
/// Represents a collection of movies created by a user.
/// </summary>
public class Collection : BaseEntity
{
  /// <summary>
  /// The name of the collection.
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// The description of the collection.
  /// </summary>
  public string? Description { get; set; }

  /// <summary>
  /// Indicates whether the collection is public or private.
  /// </summary>
  public bool IsPublic { get; set; }

  /// <summary>
  /// The user ID who owns this collection.
  /// </summary>
  public string OwnerId { get; set; } = string.Empty;

  /// <summary>
  /// The user who owns this collection.
  /// </summary>
  public virtual ApplicationUser Owner { get; set; } = null!;

  /// <summary>
  /// The movies in this collection.
  /// </summary>
  public virtual ICollection<MovieCollection> MovieCollections { get; set; } = new List<MovieCollection>();
}
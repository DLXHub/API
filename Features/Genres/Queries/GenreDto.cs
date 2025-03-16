using System;
using API.Features.Genres.Models;

namespace API.Features.Genres.Queries;

/// <summary>
/// Data transfer object for a genre
/// </summary>
public class GenreDto
{
  /// <summary>
  /// The unique identifier of the genre
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// The TMDB ID of the genre
  /// </summary>
  public int TmdbId { get; set; }

  /// <summary>
  /// The name of the genre
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// Maps a Genre entity to a GenreDto
  /// </summary>
  /// <param name="genre">The Genre entity to map</param>
  /// <returns>A GenreDto</returns>
  public static GenreDto FromEntity(Genre genre)
  {
    return new GenreDto
    {
      Id = genre.Id,
      TmdbId = genre.TmdbId,
      Name = genre.Name
    };
  }
}
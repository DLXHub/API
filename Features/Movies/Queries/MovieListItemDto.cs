using System;
using API.Features.Movies.Models;

namespace API.Features.Movies.Queries;

/// <summary>
/// Data transfer object for a movie list item
/// </summary>
public class MovieListItemDto
{
  /// <summary>
  /// The unique identifier of the movie
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// The TMDB ID of the movie
  /// </summary>
  public int TmdbId { get; set; }

  /// <summary>
  /// The title of the movie
  /// </summary>
  public string Title { get; set; } = string.Empty;

  /// <summary>
  /// The poster path for the movie
  /// </summary>
  public string? PosterPath { get; set; }

  /// <summary>
  /// The release date of the movie
  /// </summary>
  public DateTime? ReleaseDate { get; set; }

  /// <summary>
  /// The average vote for the movie
  /// </summary>
  public decimal? VoteAverage { get; set; }

  /// <summary>
  /// The popularity score of the movie
  /// </summary>
  public decimal? Popularity { get; set; }

  /// <summary>
  /// Maps a Movie entity to a MovieListItemDto
  /// </summary>
  /// <param name="movie">The Movie entity to map</param>
  /// <returns>A MovieListItemDto</returns>
  public static MovieListItemDto FromEntity(Movie movie)
  {
    return new MovieListItemDto
    {
      Id = movie.Id,
      TmdbId = movie.TmdbId,
      Title = movie.Title,
      PosterPath = movie.PosterPath,
      ReleaseDate = movie.ReleaseDate,
      VoteAverage = movie.VoteAverage,
      Popularity = movie.Popularity
    };
  }
}
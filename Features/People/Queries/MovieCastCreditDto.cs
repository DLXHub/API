using System;
using API.Features.People.Models;

namespace API.Features.People.Queries;

/// <summary>
/// Data transfer object for a movie cast credit
/// </summary>
public class MovieCastCreditDto
{
  /// <summary>
  /// The unique identifier of the credit
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// The ID of the movie
  /// </summary>
  public Guid MovieId { get; set; }

  /// <summary>
  /// The title of the movie
  /// </summary>
  public string MovieTitle { get; set; } = string.Empty;

  /// <summary>
  /// The TMDB ID of the movie
  /// </summary>
  public int MovieTmdbId { get; set; }

  /// <summary>
  /// The poster path of the movie
  /// </summary>
  public string? MoviePosterPath { get; set; }

  /// <summary>
  /// The release date of the movie
  /// </summary>
  public DateTime? MovieReleaseDate { get; set; }

  /// <summary>
  /// The character played by the person
  /// </summary>
  public string? Character { get; set; }

  /// <summary>
  /// The order of the cast member in the credits
  /// </summary>
  public int Order { get; set; }

  /// <summary>
  /// The TMDB credit ID
  /// </summary>
  public string? CreditId { get; set; }

  /// <summary>
  /// Maps a MovieCast entity to a MovieCastCreditDto
  /// </summary>
  /// <param name="movieCast">The MovieCast entity to map</param>
  /// <returns>A MovieCastCreditDto</returns>
  public static MovieCastCreditDto FromEntity(MovieCast movieCast)
  {
    return new MovieCastCreditDto
    {
      Id = movieCast.Id,
      MovieId = movieCast.MovieId,
      MovieTitle = movieCast.Movie?.Title ?? string.Empty,
      MovieTmdbId = movieCast.Movie?.TmdbId ?? 0,
      MoviePosterPath = movieCast.Movie?.PosterPath,
      MovieReleaseDate = movieCast.Movie?.ReleaseDate,
      Character = movieCast.Character,
      Order = movieCast.Order,
      CreditId = movieCast.CreditId
    };
  }
}
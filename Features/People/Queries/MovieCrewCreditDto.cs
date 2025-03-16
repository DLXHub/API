using System;
using API.Features.People.Models;

namespace API.Features.People.Queries;

/// <summary>
/// Data transfer object for a movie crew credit
/// </summary>
public class MovieCrewCreditDto
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
  /// The department the crew member worked in
  /// </summary>
  public string? Department { get; set; }

  /// <summary>
  /// The job of the crew member
  /// </summary>
  public string? Job { get; set; }

  /// <summary>
  /// The TMDB credit ID
  /// </summary>
  public string? CreditId { get; set; }

  /// <summary>
  /// Maps a MovieCrew entity to a MovieCrewCreditDto
  /// </summary>
  /// <param name="movieCrew">The MovieCrew entity to map</param>
  /// <returns>A MovieCrewCreditDto</returns>
  public static MovieCrewCreditDto FromEntity(MovieCrew movieCrew)
  {
    return new MovieCrewCreditDto
    {
      Id = movieCrew.Id,
      MovieId = movieCrew.MovieId,
      MovieTitle = movieCrew.Movie?.Title ?? string.Empty,
      MovieTmdbId = movieCrew.Movie?.TmdbId ?? 0,
      MoviePosterPath = movieCrew.Movie?.PosterPath,
      MovieReleaseDate = movieCrew.Movie?.ReleaseDate,
      Department = movieCrew.Department,
      Job = movieCrew.Job,
      CreditId = movieCrew.CreditId
    };
  }
}
using System;
using System.Collections.Generic;
using API.Features.Movies.Models;

namespace API.Features.Movies.Queries;

/// <summary>
/// Data transfer object for a movie
/// </summary>
public class MovieDto
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
  /// The original title of the movie
  /// </summary>
  public string? OriginalTitle { get; set; }

  /// <summary>
  /// The URL-friendly slug for the movie
  /// </summary>
  public string? Slug { get; set; }

  /// <summary>
  /// The overview or description of the movie
  /// </summary>
  public string? Overview { get; set; }

  /// <summary>
  /// The poster path for the movie
  /// </summary>
  public string? PosterPath { get; set; }

  /// <summary>
  /// The backdrop path for the movie
  /// </summary>
  public string? BackdropPath { get; set; }

  /// <summary>
  /// The release date of the movie
  /// </summary>
  public DateTime? ReleaseDate { get; set; }

  /// <summary>
  /// The runtime of the movie in minutes
  /// </summary>
  public int? Runtime { get; set; }

  /// <summary>
  /// The average vote for the movie
  /// </summary>
  public decimal? VoteAverage { get; set; }

  /// <summary>
  /// The number of votes for the movie
  /// </summary>
  public int? VoteCount { get; set; }

  /// <summary>
  /// The popularity score of the movie
  /// </summary>
  public decimal? Popularity { get; set; }

  /// <summary>
  /// Maps a Movie entity to a MovieDto
  /// </summary>
  /// <param name="movie">The Movie entity to map</param>
  /// <returns>A MovieDto</returns>
  public static MovieDto FromEntity(Movie movie)
  {
    return new MovieDto
    {
      Id = movie.Id,
      TmdbId = movie.TmdbId,
      Title = movie.Title,
      OriginalTitle = movie.OriginalTitle,
      Slug = movie.Slug,
      Overview = movie.Overview,
      PosterPath = movie.PosterPath,
      BackdropPath = movie.BackdropPath,
      ReleaseDate = movie.ReleaseDate,
      Runtime = movie.Runtime,
      VoteAverage = movie.VoteAverage,
      VoteCount = movie.VoteCount,
      Popularity = movie.Popularity
    };
  }
}
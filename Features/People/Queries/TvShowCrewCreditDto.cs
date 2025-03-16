using System;
using API.Features.People.Models;

namespace API.Features.People.Queries;

/// <summary>
/// Data transfer object for a TV show crew credit
/// </summary>
public class TvShowCrewCreditDto
{
  /// <summary>
  /// The unique identifier of the credit
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// The ID of the TV show
  /// </summary>
  public Guid TvShowId { get; set; }

  /// <summary>
  /// The name of the TV show
  /// </summary>
  public string TvShowName { get; set; } = string.Empty;

  /// <summary>
  /// The TMDB ID of the TV show
  /// </summary>
  public int TvShowTmdbId { get; set; }

  /// <summary>
  /// The poster path of the TV show
  /// </summary>
  public string? TvShowPosterPath { get; set; }

  /// <summary>
  /// The first air date of the TV show
  /// </summary>
  public DateTime? TvShowFirstAirDate { get; set; }

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
  /// Maps a TvShowCrew entity to a TvShowCrewCreditDto
  /// </summary>
  /// <param name="tvShowCrew">The TvShowCrew entity to map</param>
  /// <returns>A TvShowCrewCreditDto</returns>
  public static TvShowCrewCreditDto FromEntity(TvShowCrew tvShowCrew)
  {
    return new TvShowCrewCreditDto
    {
      Id = tvShowCrew.Id,
      TvShowId = tvShowCrew.TvShowId,
      TvShowName = tvShowCrew.TvShow?.Name ?? string.Empty,
      TvShowTmdbId = tvShowCrew.TvShow?.TmdbId ?? 0,
      TvShowPosterPath = tvShowCrew.TvShow?.PosterPath,
      TvShowFirstAirDate = tvShowCrew.TvShow?.FirstAirDate,
      Department = tvShowCrew.Department,
      Job = tvShowCrew.Job,
      CreditId = tvShowCrew.CreditId
    };
  }
}
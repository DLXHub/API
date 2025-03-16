using System;
using API.Features.People.Models;

namespace API.Features.People.Queries;

/// <summary>
/// Data transfer object for a TV show cast credit
/// </summary>
public class TvShowCastCreditDto
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
  /// Maps a TvShowCast entity to a TvShowCastCreditDto
  /// </summary>
  /// <param name="tvShowCast">The TvShowCast entity to map</param>
  /// <returns>A TvShowCastCreditDto</returns>
  public static TvShowCastCreditDto FromEntity(TvShowCast tvShowCast)
  {
    return new TvShowCastCreditDto
    {
      Id = tvShowCast.Id,
      TvShowId = tvShowCast.TvShowId,
      TvShowName = tvShowCast.TvShow?.Name ?? string.Empty,
      TvShowTmdbId = tvShowCast.TvShow?.TmdbId ?? 0,
      TvShowPosterPath = tvShowCast.TvShow?.PosterPath,
      TvShowFirstAirDate = tvShowCast.TvShow?.FirstAirDate,
      Character = tvShowCast.Character,
      Order = tvShowCast.Order,
      CreditId = tvShowCast.CreditId
    };
  }
}
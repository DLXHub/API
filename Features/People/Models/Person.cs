using System;
using System.Collections.Generic;
using API.Shared.Models;

namespace API.Features.People.Models;

/// <summary>
/// Represents a person (actor, director, etc.) in the system.
/// </summary>
public class Person : BaseEntity
{
  /// <summary>
  /// The TMDB ID of the person.
  /// </summary>
  public int TmdbId { get; set; }

  /// <summary>
  /// The name of the person.
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// The path to the person's profile image.
  /// </summary>
  public string? ProfilePath { get; set; }

  /// <summary>
  /// The gender of the person (1 = Female, 2 = Male, 0 = Not specified).
  /// </summary>
  public int? Gender { get; set; }

  /// <summary>
  /// The biography of the person.
  /// </summary>
  public string? Biography { get; set; }

  /// <summary>
  /// The birthday of the person.
  /// </summary>
  public DateTime? Birthday { get; set; }

  /// <summary>
  /// The deathday of the person, if applicable.
  /// </summary>
  public DateTime? Deathday { get; set; }

  /// <summary>
  /// The place of birth of the person.
  /// </summary>
  public string? PlaceOfBirth { get; set; }

  /// <summary>
  /// The popularity rating of the person.
  /// </summary>
  public decimal? Popularity { get; set; }

  /// <summary>
  /// The department the person is known for (e.g., "Acting", "Directing").
  /// </summary>
  public string? KnownForDepartment { get; set; }

  /// <summary>
  /// Alternative names the person is known by.
  /// </summary>
  public List<string>? AlsoKnownAs { get; set; }

  /// <summary>
  /// The person's homepage URL.
  /// </summary>
  public string? Homepage { get; set; }

  /// <summary>
  /// The date when the person's data was last updated.
  /// </summary>
  public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

  /// <summary>
  /// Collection of movie cast credits for this person.
  /// </summary>
  public virtual ICollection<MovieCast> MovieCastCredits { get; set; } = new List<MovieCast>();

  /// <summary>
  /// Collection of movie crew credits for this person.
  /// </summary>
  public virtual ICollection<MovieCrew> MovieCrewCredits { get; set; } = new List<MovieCrew>();

  /// <summary>
  /// Collection of TV show cast credits for this person.
  /// </summary>
  public virtual ICollection<TvShowCast> TvShowCastCredits { get; set; } = new List<TvShowCast>();

  /// <summary>
  /// Collection of TV show crew credits for this person.
  /// </summary>
  public virtual ICollection<TvShowCrew> TvShowCrewCredits { get; set; } = new List<TvShowCrew>();
}
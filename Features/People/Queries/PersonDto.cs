using System;
using System.Collections.Generic;
using API.Features.People.Models;

namespace API.Features.People.Queries;

/// <summary>
/// Data transfer object for a person
/// </summary>
public class PersonDto
{
  /// <summary>
  /// The unique identifier of the person
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// The TMDB ID of the person
  /// </summary>
  public int TmdbId { get; set; }

  /// <summary>
  /// The name of the person
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// The path to the person's profile image
  /// </summary>
  public string? ProfilePath { get; set; }

  /// <summary>
  /// The gender of the person (1 = Female, 2 = Male, 0 = Not specified)
  /// </summary>
  public int? Gender { get; set; }

  /// <summary>
  /// The biography of the person
  /// </summary>
  public string? Biography { get; set; }

  /// <summary>
  /// The birthday of the person
  /// </summary>
  public DateTime? Birthday { get; set; }

  /// <summary>
  /// The deathday of the person, if applicable
  /// </summary>
  public DateTime? Deathday { get; set; }

  /// <summary>
  /// The place of birth of the person
  /// </summary>
  public string? PlaceOfBirth { get; set; }

  /// <summary>
  /// The popularity rating of the person
  /// </summary>
  public decimal? Popularity { get; set; }

  /// <summary>
  /// The department the person is known for (e.g., "Acting", "Directing")
  /// </summary>
  public string? KnownForDepartment { get; set; }

  /// <summary>
  /// Alternative names the person is known by
  /// </summary>
  public List<string>? AlsoKnownAs { get; set; }

  /// <summary>
  /// The person's homepage URL
  /// </summary>
  public string? Homepage { get; set; }

  /// <summary>
  /// The date when the person's data was last updated
  /// </summary>
  public DateTime LastUpdated { get; set; }

  /// <summary>
  /// Maps a Person entity to a PersonDto
  /// </summary>
  /// <param name="person">The Person entity to map</param>
  /// <returns>A PersonDto</returns>
  public static PersonDto FromEntity(Person person)
  {
    return new PersonDto
    {
      Id = person.Id,
      TmdbId = person.TmdbId,
      Name = person.Name,
      ProfilePath = person.ProfilePath,
      Gender = person.Gender,
      Biography = person.Biography,
      Birthday = person.Birthday,
      Deathday = person.Deathday,
      PlaceOfBirth = person.PlaceOfBirth,
      Popularity = person.Popularity,
      KnownForDepartment = person.KnownForDepartment,
      AlsoKnownAs = person.AlsoKnownAs,
      Homepage = person.Homepage,
      LastUpdated = person.LastUpdated
    };
  }
}
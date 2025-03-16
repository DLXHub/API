using System;
using API.Features.People.Models;

namespace API.Features.People.Queries;

/// <summary>
/// Data transfer object for a person in a list
/// </summary>
public class PersonListItemDto
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
  /// The department the person is known for (e.g., "Acting", "Directing")
  /// </summary>
  public string? KnownForDepartment { get; set; }

  /// <summary>
  /// The popularity rating of the person
  /// </summary>
  public decimal? Popularity { get; set; }

  /// <summary>
  /// Maps a Person entity to a PersonListItemDto
  /// </summary>
  /// <param name="person">The Person entity to map</param>
  /// <returns>A PersonListItemDto</returns>
  public static PersonListItemDto FromEntity(Person person)
  {
    return new PersonListItemDto
    {
      Id = person.Id,
      TmdbId = person.TmdbId,
      Name = person.Name,
      ProfilePath = person.ProfilePath,
      KnownForDepartment = person.KnownForDepartment,
      Popularity = person.Popularity
    };
  }
}
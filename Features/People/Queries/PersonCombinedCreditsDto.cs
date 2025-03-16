using API.Features.People.Models;

namespace API.Features.People.Queries;

/// <summary>
/// Data transfer object for a person's combined credits
/// </summary>
public class PersonCombinedCreditsDto
{
  /// <summary>
  /// The unique identifier of the person
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// The name of the person
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// The movie cast credits
  /// </summary>
  public List<MovieCastCreditDto> MovieCast { get; set; } = new List<MovieCastCreditDto>();

  /// <summary>
  /// The movie crew credits
  /// </summary>
  public List<MovieCrewCreditDto> MovieCrew { get; set; } = new List<MovieCrewCreditDto>();

  /// <summary>
  /// The TV show cast credits
  /// </summary>
  public List<TvShowCastCreditDto> TvCast { get; set; } = new List<TvShowCastCreditDto>();

  /// <summary>
  /// The TV show crew credits
  /// </summary>
  public List<TvShowCrewCreditDto> TvCrew { get; set; } = new List<TvShowCrewCreditDto>();

  /// <summary>
  /// Maps a Person entity with its credits to a PersonCombinedCreditsDto
  /// </summary>
  /// <param name="person">The Person entity to map</param>
  /// <param name="movieCast">The movie cast credits of the person</param>
  /// <param name="movieCrew">The movie crew credits of the person</param>
  /// <param name="tvCast">The TV show cast credits of the person</param>
  /// <param name="tvCrew">The TV show crew credits of the person</param>
  /// <returns>A PersonCombinedCreditsDto</returns>
  public static PersonCombinedCreditsDto FromEntity(
      Person person,
      IEnumerable<MovieCast> movieCast,
      IEnumerable<MovieCrew> movieCrew,
      IEnumerable<TvShowCast> tvCast,
      IEnumerable<TvShowCrew> tvCrew)
  {
    var dto = new PersonCombinedCreditsDto
    {
      Id = person.Id,
      Name = person.Name
    };

    foreach (var cast in movieCast)
    {
      dto.MovieCast.Add(MovieCastCreditDto.FromEntity(cast));
    }

    foreach (var crew in movieCrew)
    {
      dto.MovieCrew.Add(MovieCrewCreditDto.FromEntity(crew));
    }

    foreach (var cast in tvCast)
    {
      dto.TvCast.Add(TvShowCastCreditDto.FromEntity(cast));
    }

    foreach (var crew in tvCrew)
    {
      dto.TvCrew.Add(TvShowCrewCreditDto.FromEntity(crew));
    }

    return dto;
  }
}
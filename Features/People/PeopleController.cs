using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Features.People.Commands;
using API.Features.People.Models;
using API.Features.People.Queries;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.People;

[ApiController]
[Route("api/people")]
public class PeopleController : ControllerBase
{
  private readonly IMediator _mediator;

  public PeopleController(IMediator mediator)
  {
    _mediator = mediator;
  }

  /// <summary>
  /// Gets a person by ID
  /// </summary>
  /// <param name="id">The ID of the person to retrieve</param>
  /// <returns>The person details</returns>
  [HttpGet("{id}")]
  [AllowAnonymous]
  public async Task<ActionResult<PersonDto>> GetPerson(Guid id)
  {
    var query = new GetPersonQuery { Id = id };
    var result = await _mediator.Send(query);

    if (result == null)
    {
      return NotFound();
    }

    return Ok(result);
  }

  /// <summary>
  /// Gets a person by TMDB ID
  /// </summary>
  /// <param name="tmdbId">The TMDB ID of the person to retrieve</param>
  /// <returns>The person details</returns>
  [HttpGet("tmdb/{tmdbId}")]
  [AllowAnonymous]
  public async Task<ActionResult<PersonDto>> GetPersonByTmdbId(int tmdbId)
  {
    var query = new GetPersonByTmdbIdQuery { TmdbId = tmdbId };
    var result = await _mediator.Send(query);

    if (result == null)
    {
      return NotFound();
    }

    return Ok(result);
  }

  /// <summary>
  /// Gets a list of people with optional filtering and pagination
  /// </summary>
  /// <param name="query">The query parameters</param>
  /// <returns>A list of people</returns>
  [HttpGet]
  [AllowAnonymous]
  public async Task<ActionResult<PaginatedList<PersonListItemDto>>> GetPeople([FromQuery] GetPeopleQuery query)
  {
    var result = await _mediator.Send(query);
    return Ok(result);
  }

  /// <summary>
  /// Gets the movie cast credits for a person
  /// </summary>
  /// <param name="id">The ID of the person</param>
  /// <returns>A list of movie cast credits</returns>
  [HttpGet("{id}/movie-cast")]
  [AllowAnonymous]
  public async Task<ActionResult<List<MovieCastCreditDto>>> GetPersonMovieCastCredits(Guid id)
  {
    var query = new GetPersonMovieCastCreditsQuery { PersonId = id };
    var result = await _mediator.Send(query);
    return Ok(result);
  }

  /// <summary>
  /// Gets the movie crew credits for a person
  /// </summary>
  /// <param name="id">The ID of the person</param>
  /// <returns>A list of movie crew credits</returns>
  [HttpGet("{id}/movie-crew")]
  [AllowAnonymous]
  public async Task<ActionResult<List<MovieCrewCreditDto>>> GetPersonMovieCrewCredits(Guid id)
  {
    var query = new GetPersonMovieCrewCreditsQuery { PersonId = id };
    var result = await _mediator.Send(query);
    return Ok(result);
  }

  /// <summary>
  /// Gets the TV show cast credits for a person
  /// </summary>
  /// <param name="id">The ID of the person</param>
  /// <returns>A list of TV show cast credits</returns>
  [HttpGet("{id}/tv-cast")]
  [AllowAnonymous]
  public async Task<ActionResult<List<TvShowCastCreditDto>>> GetPersonTvShowCastCredits(Guid id)
  {
    var query = new GetPersonTvShowCastCreditsQuery { PersonId = id };
    var result = await _mediator.Send(query);
    return Ok(result);
  }

  /// <summary>
  /// Gets the TV show crew credits for a person
  /// </summary>
  /// <param name="id">The ID of the person</param>
  /// <returns>A list of TV show crew credits</returns>
  [HttpGet("{id}/tv-crew")]
  [AllowAnonymous]
  public async Task<ActionResult<List<TvShowCrewCreditDto>>> GetPersonTvShowCrewCredits(Guid id)
  {
    var query = new GetPersonTvShowCrewCreditsQuery { PersonId = id };
    var result = await _mediator.Send(query);
    return Ok(result);
  }

  /// <summary>
  /// Gets all credits for a person (movie and TV, cast and crew)
  /// </summary>
  /// <param name="id">The ID of the person</param>
  /// <returns>All credits for the person</returns>
  [HttpGet("{id}/combined-credits")]
  [AllowAnonymous]
  public async Task<ActionResult<PersonCombinedCreditsDto>> GetPersonCombinedCredits(Guid id)
  {
    var query = new GetPersonCombinedCreditsQuery { PersonId = id };
    var result = await _mediator.Send(query);

    if (result == null)
    {
      return NotFound();
    }

    return Ok(result);
  }

  /// <summary>
  /// Imports a person from TMDB
  /// </summary>
  /// <param name="command">The import command</param>
  /// <returns>The imported person</returns>
  [HttpPost("import")]
  [Authorize(Roles = "Admin")]
  public async Task<ActionResult<PersonDto>> ImportPerson(ImportPersonCommand command)
  {
    var result = await _mediator.Send(command);
    return Ok(result);
  }

  /// <summary>
  /// Imports a person's credits from TMDB
  /// </summary>
  /// <param name="id">The ID of the person</param>
  /// <returns>The imported credits</returns>
  [HttpPost("{id}/import-credits")]
  [Authorize(Roles = "Admin")]
  public async Task<ActionResult<PersonCombinedCreditsDto>> ImportPersonCredits(Guid id)
  {
    var command = new ImportPersonCreditsCommand { PersonId = id };
    var result = await _mediator.Send(command);

    if (result == null)
    {
      return NotFound();
    }

    return Ok(result);
  }
}
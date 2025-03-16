using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Features.Genres.Commands;
using API.Features.Genres.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Genres;

/// <summary>
/// Controller for genre-related operations
/// </summary>
[ApiController]
[Route("api/genres")]
public class GenresController : ControllerBase
{
  private readonly IMediator _mediator;

  /// <summary>
  /// Initializes a new instance of the GenresController class
  /// </summary>
  /// <param name="mediator">The mediator instance</param>
  public GenresController(IMediator mediator)
  {
    _mediator = mediator;
  }

  /// <summary>
  /// Gets a genre by ID
  /// </summary>
  /// <param name="id">The ID of the genre to retrieve</param>
  /// <param name="mediaType">Optional filter by media type ("movie" or "tv")</param>
  /// <returns>The genre details</returns>
  [HttpGet("{id}")]
  [AllowAnonymous]
  public async Task<ActionResult<GenreDto>> GetGenre(Guid id, [FromQuery] string? mediaType = null)
  {
    var query = new GetGenreQuery { Id = id, MediaType = mediaType };
    var result = await _mediator.Send(query);

    if (result == null)
    {
      return NotFound();
    }

    return Ok(result);
  }

  /// <summary>
  /// Gets all genres
  /// </summary>
  /// <param name="sortByName">Whether to sort by name (true) or ID (false)</param>
  /// <param name="mediaType">Optional filter by media type ("movie" or "tv")</param>
  /// <returns>A list of all genres</returns>
  [HttpGet]
  [AllowAnonymous]
  public async Task<ActionResult<List<GenreDto>>> GetGenres(
      [FromQuery] bool sortByName = true,
      [FromQuery] string? mediaType = null)
  {
    var query = new GetGenresQuery { SortByName = sortByName, MediaType = mediaType };
    var result = await _mediator.Send(query);
    return Ok(result);
  }

  /// <summary>
  /// Imports genres from TMDB
  /// </summary>
  /// <param name="type">The type of genres to import (movie or tv)</param>
  /// <returns>A list of all genres after import</returns>
  [HttpPost("import")]
  [Authorize(Roles = "Admin")]
  public async Task<ActionResult<List<GenreDto>>> ImportGenres([FromQuery] string type = "movie")
  {
    var command = new ImportGenresCommand { Type = type };
    var result = await _mediator.Send(command);
    return Ok(result);
  }
}
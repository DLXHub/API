using System;
using System.Threading.Tasks;
using API.Features.Movies.Queries;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Movies;

/// <summary>
/// Controller for movie-related operations
/// </summary>
[ApiController]
[Route("api/movies")]
public class MoviesController : ControllerBase
{
  private readonly IMediator _mediator;

  /// <summary>
  /// Initializes a new instance of the MoviesController class
  /// </summary>
  /// <param name="mediator">The mediator instance</param>
  public MoviesController(IMediator mediator)
  {
    _mediator = mediator;
  }

  /// <summary>
  /// Gets a movie by ID
  /// </summary>
  /// <param name="id">The ID of the movie to retrieve</param>
  /// <returns>The movie details</returns>
  [HttpGet("{id}")]
  [AllowAnonymous]
  public async Task<ActionResult<MovieDto>> GetMovie(Guid id)
  {
    var query = new GetMovieQuery { Id = id };
    var result = await _mediator.Send(query);

    if (result == null)
    {
      return NotFound();
    }

    return Ok(result);
  }

  /// <summary>
  /// Gets a movie by slug
  /// </summary>
  /// <param name="slug">The slug of the movie to retrieve</param>
  /// <returns>The movie details</returns>
  [HttpGet("by-slug/{slug}")]
  [AllowAnonymous]
  public async Task<ActionResult<MovieDto>> GetMovieBySlug(string slug)
  {
    var query = new GetMovieBySlugQuery { Slug = slug };
    var result = await _mediator.Send(query);

    if (result == null)
    {
      return NotFound();
    }

    return Ok(result);
  }

  /// <summary>
  /// Gets a movie by TMDB ID
  /// </summary>
  /// <param name="tmdbId">The TMDB ID of the movie to retrieve</param>
  /// <returns>The movie details</returns>
  [HttpGet("tmdb/{tmdbId}")]
  [AllowAnonymous]
  public async Task<ActionResult<MovieDto>> GetMovieByTmdbId(int tmdbId)
  {
    var query = new GetMovieByTmdbIdQuery { TmdbId = tmdbId };
    var result = await _mediator.Send(query);

    if (result == null)
    {
      return NotFound();
    }

    return Ok(result);
  }

  /// <summary>
  /// Gets a list of movies with optional filtering and pagination
  /// </summary>
  /// <param name="query">The query parameters</param>
  /// <returns>A paginated list of movies</returns>
  [HttpGet]
  [AllowAnonymous]
  public async Task<ActionResult<PaginatedList<MovieListItemDto>>> GetMovies([FromQuery] GetMoviesQuery query)
  {
    var result = await _mediator.Send(query);
    return Ok(result);
  }
}
using API.Features.TvShows.Commands.ImportTvShow;
using API.Features.TvShows.Queries.GetSeasonEpisodes;
using API.Features.TvShows.Queries.GetTvShow;
using API.Features.TvShows.Queries.GetTvShowBySlug;
using API.Features.TvShows.Queries.GetTvShowEpisodesBySeasonNumber;
using API.Features.TvShows.Queries.GetTvShowsBySeason;
using API.Features.TvShows.Queries.GetTvShowSeasons;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.TvShows;

/// <summary>
/// Controller for managing TV shows.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TvShowsController : ControllerBase
{
  private readonly IMediator _mediator;

  /// <summary>
  /// Initializes a new instance of the TvShowsController class.
  /// </summary>
  /// <param name="mediator">The mediator.</param>
  public TvShowsController(IMediator mediator)
  {
    _mediator = mediator;
  }

  /// <summary>
  /// Gets a TV show by ID.
  /// </summary>
  /// <param name="id">The ID of the TV show.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The TV show.</returns>
  [HttpGet("{id}")]
  [ProducesResponseType(typeof(ApiResponse<TvShowDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<TvShowDto>), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<ApiResponse<TvShowDto>>> GetTvShow(
      [FromRoute] Guid id,
      CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new GetTvShowQuery { Id = id }, cancellationToken);

    if (!result.Success)
    {
      return NotFound(result);
    }

    return Ok(result);
  }

  /// <summary>
  /// Gets a TV show by slug.
  /// </summary>
  /// <param name="slug">The slug of the TV show.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The TV show.</returns>
  [HttpGet("by-slug/{slug}")]
  [ProducesResponseType(typeof(ApiResponse<TvShowDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<TvShowDto>), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<ApiResponse<TvShowDto>>> GetTvShowBySlug(
      [FromRoute] string slug,
      CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new GetTvShowBySlugQuery { Slug = slug }, cancellationToken);

    if (!result.Success)
    {
      return NotFound(result);
    }

    return Ok(result);
  }

  /// <summary>
  /// Gets TV shows filtered by season number.
  /// </summary>
  /// <param name="seasonNumber">The season number to filter by. If null, returns all TV shows.</param>
  /// <param name="exactMatch">Whether to include seasons with the specified number only (true) or all seasons up to and including the specified number (false).</param>
  /// <param name="hasEpisodes">Whether to include TV shows that have episodes in the specified season (true) or just having the season is enough (false).</param>
  /// <param name="minEpisodeCount">The minimum number of episodes a season should have to be included (only used when hasEpisodes is true).</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of TV shows.</returns>
  [HttpGet("by-season")]
  [ProducesResponseType(typeof(ApiResponse<List<TvShowDto>>), StatusCodes.Status200OK)]
  public async Task<ActionResult<ApiResponse<List<TvShowDto>>>> GetTvShowsBySeason(
      [FromQuery] int? seasonNumber = null,
      [FromQuery] bool exactMatch = true,
      [FromQuery] bool hasEpisodes = false,
      [FromQuery] int? minEpisodeCount = null,
      CancellationToken cancellationToken = default)
  {
    var query = new GetTvShowsBySeasonQuery
    {
      SeasonNumber = seasonNumber,
      ExactMatch = exactMatch,
      HasEpisodes = hasEpisodes,
      MinEpisodeCount = minEpisodeCount
    };

    var result = await _mediator.Send(query, cancellationToken);
    return Ok(result);
  }

  /// <summary>
  /// Gets all episodes of a specific season number for a TV show.
  /// </summary>
  /// <param name="id">The ID of the TV show.</param>
  /// <param name="seasonNumber">The season number to get episodes for.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of episodes.</returns>
  [HttpGet("{id}/season/{seasonNumber}/episodes")]
  [ProducesResponseType(typeof(ApiResponse<List<EpisodeDto>>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<List<EpisodeDto>>), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<ApiResponse<List<EpisodeDto>>>> GetTvShowEpisodesBySeasonNumber(
      [FromRoute] Guid id,
      [FromRoute] int seasonNumber,
      CancellationToken cancellationToken)
  {
    var query = new GetTvShowEpisodesBySeasonNumberQuery
    {
      TvShowId = id,
      SeasonNumber = seasonNumber
    };

    var result = await _mediator.Send(query, cancellationToken);

    if (!result.Success)
    {
      return NotFound(result);
    }

    return Ok(result);
  }

  /// <summary>
  /// Gets all seasons of a TV show.
  /// </summary>
  /// <param name="id">The ID of the TV show.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of seasons.</returns>
  [HttpGet("{id}/seasons")]
  [ProducesResponseType(typeof(ApiResponse<List<SeasonDto>>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<List<SeasonDto>>), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<ApiResponse<List<SeasonDto>>>> GetTvShowSeasons(
      [FromRoute] Guid id,
      CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new GetTvShowSeasonsQuery { TvShowId = id }, cancellationToken);

    if (!result.Success)
    {
      return NotFound(result);
    }

    return Ok(result);
  }

  /// <summary>
  /// Gets all episodes of a season.
  /// </summary>
  /// <param name="id">The ID of the TV show.</param>
  /// <param name="seasonId">The ID of the season.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of episodes.</returns>
  [HttpGet("{id}/seasons/{seasonId}/episodes")]
  [ProducesResponseType(typeof(ApiResponse<List<EpisodeDto>>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<List<EpisodeDto>>), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<ApiResponse<List<EpisodeDto>>>> GetSeasonEpisodes(
      [FromRoute] Guid id,
      [FromRoute] Guid seasonId,
      CancellationToken cancellationToken)
  {
    // The TV show ID is not used in the query, but it's included in the route for RESTful design
    var result = await _mediator.Send(new GetSeasonEpisodesQuery { SeasonId = seasonId }, cancellationToken);

    if (!result.Success)
    {
      return NotFound(result);
    }

    return Ok(result);
  }

  /// <summary>
  /// Imports a TV show from TMDB.
  /// </summary>
  /// <param name="command">The command to import a TV show.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The imported TV show.</returns>
  [HttpPost("import")]
  [Authorize]
  [ProducesResponseType(typeof(ApiResponse<TvShowDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<TvShowDto>), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse<TvShowDto>), StatusCodes.Status401Unauthorized)]
  public async Task<ActionResult<ApiResponse<TvShowDto>>> ImportTvShow(
      [FromBody] ImportTvShowCommand command,
      CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(command, cancellationToken);
    return result.Success ? Ok(result) : BadRequest(result);
  }
}
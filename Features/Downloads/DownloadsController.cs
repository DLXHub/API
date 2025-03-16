using API.Features.Downloads.Commands.CreateDownload;
using API.Features.Downloads.Models;
using API.Features.Downloads.Queries.GetDownloads;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Downloads;

/// <summary>
/// Controller for managing downloads.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DownloadsController : ControllerBase
{
  private readonly IMediator _mediator;

  /// <summary>
  /// Initializes a new instance of the DownloadsController class.
  /// </summary>
  /// <param name="mediator">The mediator instance.</param>
  public DownloadsController(IMediator mediator)
  {
    _mediator = mediator;
  }

  /// <summary>
  /// Creates a new download.
  /// </summary>
  /// <param name="command">The command to create a download.</param>
  /// <returns>The created download.</returns>
  [HttpPost]
  [ProducesResponseType(typeof(ApiResponse<DownloadDto>), 200)]
  public async Task<ActionResult<ApiResponse<DownloadDto>>> CreateDownload(CreateDownloadCommand command)
  {
    var result = await _mediator.Send(command);
    return Ok(result);
  }

  /// <summary>
  /// Gets a paginated list of downloads with optional filtering.
  /// </summary>
  /// <param name="mediaType">The type of media to filter by.</param>
  /// <param name="mediaId">The ID of the specific media item to get downloads for.</param>
  /// <param name="language">The language to filter by.</param>
  /// <param name="quality">The quality to filter by.</param>
  /// <param name="pageNumber">The page number (1-based).</param>
  /// <param name="pageSize">The number of items per page.</param>
  /// <returns>A paginated list of downloads.</returns>
  [HttpGet]
  [ProducesResponseType(typeof(ApiResponse<PaginatedList<DownloadDto>>), 200)]
  public async Task<ActionResult<ApiResponse<PaginatedList<DownloadDto>>>> GetDownloads(
      [FromQuery] MediaType? mediaType,
      [FromQuery] Guid? mediaId,
      [FromQuery] string? language,
      [FromQuery] string? quality,
      [FromQuery] int pageNumber = 1,
      [FromQuery] int pageSize = 10)
  {
    var query = new GetDownloadsQuery
    {
      MediaType = mediaType,
      MediaId = mediaId,
      Language = language,
      Quality = quality,
      PageNumber = pageNumber,
      PageSize = pageSize
    };

    var result = await _mediator.Send(query);
    return Ok(result);
  }

  /// <summary>
  /// Gets downloads for a specific movie.
  /// </summary>
  /// <param name="movieId">The ID of the movie.</param>
  /// <param name="language">The language to filter by.</param>
  /// <param name="quality">The quality to filter by.</param>
  /// <param name="pageNumber">The page number (1-based).</param>
  /// <param name="pageSize">The number of items per page.</param>
  /// <returns>A paginated list of downloads for the movie.</returns>
  [HttpGet("movies/{movieId}")]
  [ProducesResponseType(typeof(ApiResponse<PaginatedList<DownloadDto>>), 200)]
  public async Task<ActionResult<ApiResponse<PaginatedList<DownloadDto>>>> GetMovieDownloads(
      Guid movieId,
      [FromQuery] string? language,
      [FromQuery] string? quality,
      [FromQuery] int pageNumber = 1,
      [FromQuery] int pageSize = 10)
  {
    var query = new GetDownloadsQuery
    {
      MediaType = MediaType.Movie,
      MediaId = movieId,
      Language = language,
      Quality = quality,
      PageNumber = pageNumber,
      PageSize = pageSize
    };

    var result = await _mediator.Send(query);
    return Ok(result);
  }

  /// <summary>
  /// Gets downloads for a specific TV show.
  /// </summary>
  /// <param name="tvShowId">The ID of the TV show.</param>
  /// <param name="language">The language to filter by.</param>
  /// <param name="quality">The quality to filter by.</param>
  /// <param name="pageNumber">The page number (1-based).</param>
  /// <param name="pageSize">The number of items per page.</param>
  /// <returns>A paginated list of downloads for the TV show.</returns>
  [HttpGet("tvshows/{tvShowId}")]
  [ProducesResponseType(typeof(ApiResponse<PaginatedList<DownloadDto>>), 200)]
  public async Task<ActionResult<ApiResponse<PaginatedList<DownloadDto>>>> GetTvShowDownloads(
      Guid tvShowId,
      [FromQuery] string? language,
      [FromQuery] string? quality,
      [FromQuery] int pageNumber = 1,
      [FromQuery] int pageSize = 10)
  {
    var query = new GetDownloadsQuery
    {
      MediaType = MediaType.TvShow,
      MediaId = tvShowId,
      Language = language,
      Quality = quality,
      PageNumber = pageNumber,
      PageSize = pageSize
    };

    var result = await _mediator.Send(query);
    return Ok(result);
  }

  /// <summary>
  /// Gets downloads for a specific season.
  /// </summary>
  /// <param name="seasonId">The ID of the season.</param>
  /// <param name="language">The language to filter by.</param>
  /// <param name="quality">The quality to filter by.</param>
  /// <param name="pageNumber">The page number (1-based).</param>
  /// <param name="pageSize">The number of items per page.</param>
  /// <returns>A paginated list of downloads for the season.</returns>
  [HttpGet("seasons/{seasonId}")]
  [ProducesResponseType(typeof(ApiResponse<PaginatedList<DownloadDto>>), 200)]
  public async Task<ActionResult<ApiResponse<PaginatedList<DownloadDto>>>> GetSeasonDownloads(
      Guid seasonId,
      [FromQuery] string? language,
      [FromQuery] string? quality,
      [FromQuery] int pageNumber = 1,
      [FromQuery] int pageSize = 10)
  {
    var query = new GetDownloadsQuery
    {
      MediaType = MediaType.Season,
      MediaId = seasonId,
      Language = language,
      Quality = quality,
      PageNumber = pageNumber,
      PageSize = pageSize
    };

    var result = await _mediator.Send(query);
    return Ok(result);
  }

  /// <summary>
  /// Gets downloads for a specific episode.
  /// </summary>
  /// <param name="episodeId">The ID of the episode.</param>
  /// <param name="language">The language to filter by.</param>
  /// <param name="quality">The quality to filter by.</param>
  /// <param name="pageNumber">The page number (1-based).</param>
  /// <param name="pageSize">The number of items per page.</param>
  /// <returns>A paginated list of downloads for the episode.</returns>
  [HttpGet("episodes/{episodeId}")]
  [ProducesResponseType(typeof(ApiResponse<PaginatedList<DownloadDto>>), 200)]
  public async Task<ActionResult<ApiResponse<PaginatedList<DownloadDto>>>> GetEpisodeDownloads(
      Guid episodeId,
      [FromQuery] string? language,
      [FromQuery] string? quality,
      [FromQuery] int pageNumber = 1,
      [FromQuery] int pageSize = 10)
  {
    var query = new GetDownloadsQuery
    {
      MediaType = MediaType.Episode,
      MediaId = episodeId,
      Language = language,
      Quality = quality,
      PageNumber = pageNumber,
      PageSize = pageSize
    };

    var result = await _mediator.Send(query);
    return Ok(result);
  }
}
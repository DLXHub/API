using API.Features.Pages.Commands.CreatePage;
using API.Features.Pages.Commands.PublishPage;
using API.Features.Pages.Commands.UpdatePage;
using API.Features.Pages.Models;
using API.Features.Pages.Queries.GetPageByLinkTarget;
using API.Features.Pages.Queries.GetPageBySlug;
using API.Features.Pages.Queries.GetPages;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Pages;

/// <summary>
/// Controller for managing pages.
/// </summary>
[ApiController]
[Route("api/pages")]
public class PagesController : ControllerBase
{
  private readonly IMediator _mediator;

  /// <summary>
  /// Initializes a new instance of the PagesController class.
  /// </summary>
  public PagesController(IMediator mediator)
  {
    _mediator = mediator;
  }

  /// <summary>
  /// Creates a new page.
  /// </summary>
  /// <param name="command">The command to create a page.</param>
  /// <returns>The created page.</returns>
  [HttpPost]
  [Authorize(Roles = "Admin")]
  [ProducesResponseType(typeof(ApiResponse<PageDto>), 200)]
  public async Task<ActionResult<ApiResponse<PageDto>>> Create([FromBody] CreatePageCommand command)
  {
    var result = await _mediator.Send(command);
    return Ok(result);
  }

  /// <summary>
  /// Updates an existing page.
  /// </summary>
  /// <param name="id">The ID of the page to update.</param>
  /// <param name="command">The command to update the page.</param>
  /// <returns>The updated page.</returns>
  [HttpPut("{id}")]
  [Authorize(Roles = "Admin")]
  [ProducesResponseType(typeof(ApiResponse<PageDto>), 200)]
  public async Task<ActionResult<ApiResponse<PageDto>>> Update(Guid id, [FromBody] UpdatePageCommand command)
  {
    if (id != command.Id)
    {
      return BadRequest("ID mismatch");
    }

    var result = await _mediator.Send(command);
    return Ok(result);
  }

  /// <summary>
  /// Publishes a page.
  /// </summary>
  /// <param name="id">The ID of the page to publish.</param>
  /// <returns>The published page.</returns>
  [HttpPost("{id}/publish")]
  [Authorize(Roles = "Admin")]
  [ProducesResponseType(typeof(ApiResponse<PageDto>), 200)]
  public async Task<ActionResult<ApiResponse<PageDto>>> Publish(Guid id)
  {
    var command = new PublishPageCommand { Id = id };
    var result = await _mediator.Send(command);
    return Ok(result);
  }

  /// <summary>
  /// Gets a page by its slug.
  /// </summary>
  /// <param name="slug">The slug of the page.</param>
  /// <param name="includeDrafts">Whether to include draft versions in the search.</param>
  /// <returns>The requested page.</returns>
  [HttpGet("by-slug/{slug}")]
  [ProducesResponseType(typeof(ApiResponse<PageDto>), 200)]
  public async Task<ActionResult<ApiResponse<PageDto>>> GetBySlug(string slug, [FromQuery] bool includeDrafts = false)
  {
    var query = new GetPageBySlugQuery { Slug = slug, IncludeDrafts = includeDrafts };
    var result = await _mediator.Send(query);
    return Ok(result);
  }

  /// <summary>
  /// Gets a page by its link target.
  /// </summary>
  /// <param name="linkTarget">The link target of the page.</param>
  /// <param name="includeDrafts">Whether to include draft versions in the search.</param>
  /// <returns>The requested page.</returns>
  [HttpGet("by-link-target/{linkTarget}")]
  [ProducesResponseType(typeof(ApiResponse<PageDto>), 200)]
  public async Task<ActionResult<ApiResponse<PageDto>>> GetByLinkTarget(string linkTarget, [FromQuery] bool includeDrafts = false)
  {
    var query = new GetPageByLinkTargetQuery { LinkTarget = linkTarget, IncludeDrafts = includeDrafts };
    var result = await _mediator.Send(query);
    return Ok(result);
  }

  /// <summary>
  /// Gets a paginated list of pages.
  /// </summary>
  /// <param name="pageNumber">The page number to retrieve (1-based).</param>
  /// <param name="pageSize">The number of items per page.</param>
  /// <param name="searchTerm">Optional search term to filter pages.</param>
  /// <param name="status">Optional status filter.</param>
  /// <param name="includeDrafts">Whether to include draft versions.</param>
  /// <returns>A paginated list of pages.</returns>
  [HttpGet]
  [ProducesResponseType(typeof(ApiResponse<PaginatedList<PageDto>>), 200)]
  public async Task<ActionResult<ApiResponse<PaginatedList<PageDto>>>> GetPages(
      [FromQuery] int pageNumber = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string? searchTerm = null,
      [FromQuery] PageStatus? status = null,
      [FromQuery] bool includeDrafts = false)
  {
    var query = new GetPagesQuery
    {
      PageNumber = pageNumber,
      PageSize = pageSize,
      SearchTerm = searchTerm,
      Status = status,
      IncludeDrafts = includeDrafts
    };

    var result = await _mediator.Send(query);
    return Ok(result);
  }
}
using API.Features.Collections.Commands.AddMovieToCollection;
using API.Features.Collections.Commands.CreateCollection;
using API.Features.Collections.Queries.GetCollection;
using API.Features.Collections.Queries.GetCollectionMovies;
using API.Features.Collections.Queries.GetUserCollections;
using API.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Collections;

/// <summary>
/// Controller for managing movie collections.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CollectionsController : ControllerBase
{
  private readonly IMediator _mediator;

  /// <summary>
  /// Initializes a new instance of the CollectionsController class.
  /// </summary>
  /// <param name="mediator">The mediator.</param>
  public CollectionsController(IMediator mediator)
  {
    _mediator = mediator;
  }

  /// <summary>
  /// Creates a new collection.
  /// </summary>
  /// <param name="command">The command to create a collection.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The created collection.</returns>
  [HttpPost]
  [ProducesResponseType(typeof(ApiResponse<CollectionDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<CollectionDto>), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse<CollectionDto>), StatusCodes.Status401Unauthorized)]
  public async Task<ActionResult<ApiResponse<CollectionDto>>> CreateCollection(
      [FromBody] CreateCollectionCommand command,
      CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(command, cancellationToken);
    return result.Success ? Ok(result) : BadRequest(result);
  }

  /// <summary>
  /// Gets a collection by ID.
  /// </summary>
  /// <param name="id">The ID of the collection.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The collection.</returns>
  [HttpGet("{id}")]
  [ProducesResponseType(typeof(ApiResponse<CollectionDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<CollectionDto>), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse<CollectionDto>), StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(typeof(ApiResponse<CollectionDto>), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<ApiResponse<CollectionDto>>> GetCollection(
      [FromRoute] Guid id,
      CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new GetCollectionQuery { Id = id }, cancellationToken);

    if (!result.Success)
    {
      return result.Message == "Collection not found."
          ? NotFound(result)
          : BadRequest(result);
    }

    return Ok(result);
  }

  /// <summary>
  /// Gets all collections for the current user.
  /// </summary>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of collections.</returns>
  [HttpGet]
  [ProducesResponseType(typeof(ApiResponse<List<CollectionDto>>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<List<CollectionDto>>), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse<List<CollectionDto>>), StatusCodes.Status401Unauthorized)]
  public async Task<ActionResult<ApiResponse<List<CollectionDto>>>> GetUserCollections(
      CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new GetUserCollectionsQuery(), cancellationToken);
    return result.Success ? Ok(result) : BadRequest(result);
  }

  /// <summary>
  /// Gets all movies in a collection.
  /// </summary>
  /// <param name="id">The ID of the collection.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The collection with its movies.</returns>
  [HttpGet("{id}/movies")]
  [ProducesResponseType(typeof(ApiResponse<CollectionMoviesDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<CollectionMoviesDto>), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse<CollectionMoviesDto>), StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(typeof(ApiResponse<CollectionMoviesDto>), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<ApiResponse<CollectionMoviesDto>>> GetCollectionMovies(
      [FromRoute] Guid id,
      CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new GetCollectionMoviesQuery { CollectionId = id }, cancellationToken);

    if (!result.Success)
    {
      return result.Message == "Collection not found."
          ? NotFound(result)
          : BadRequest(result);
    }

    return Ok(result);
  }

  /// <summary>
  /// Adds a movie to a collection.
  /// </summary>
  /// <param name="id">The ID of the collection.</param>
  /// <param name="command">The command to add a movie to a collection.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>True if the movie was added successfully.</returns>
  [HttpPost("{id}/movies")]
  [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<ApiResponse<bool>>> AddMovieToCollection(
      [FromRoute] Guid id,
      [FromBody] AddMovieToCollectionCommand command,
      CancellationToken cancellationToken)
  {
    // Ensure the collection ID in the route matches the one in the command
    if (id != command.CollectionId)
    {
      command = command with { CollectionId = id };
    }

    var result = await _mediator.Send(command, cancellationToken);

    if (!result.Success)
    {
      return result.Message == "Collection not found."
          ? NotFound(result)
          : BadRequest(result);
    }

    return Ok(result);
  }
}
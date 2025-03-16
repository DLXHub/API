using API.Features.Collections.Commands.CreateCollection;
using API.Shared.Infrastructure;
using API.Shared.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Collections.Queries.GetCollection;

/// <summary>
/// Query to get a collection by ID.
/// </summary>
public record GetCollectionQuery : IRequest<ApiResponse<CollectionDto>>
{
  /// <summary>
  /// The ID of the collection to retrieve.
  /// </summary>
  public Guid Id { get; init; }
}

/// <summary>
/// Validator for the GetCollectionQuery.
/// </summary>
public class GetCollectionQueryValidator : AbstractValidator<GetCollectionQuery>
{
  /// <summary>
  /// Initializes a new instance of the GetCollectionQueryValidator class.
  /// </summary>
  public GetCollectionQueryValidator()
  {
    RuleFor(x => x.Id)
        .NotEmpty().WithMessage("Collection ID is required.");
  }
}

/// <summary>
/// Handler for the GetCollectionQuery.
/// </summary>
public class GetCollectionQueryHandler : IRequestHandler<GetCollectionQuery, ApiResponse<CollectionDto>>
{
  private readonly ApplicationDbContext _context;
  private readonly IHttpContextAccessor _httpContextAccessor;

  /// <summary>
  /// Initializes a new instance of the GetCollectionQueryHandler class.
  /// </summary>
  /// <param name="context">The database context.</param>
  /// <param name="httpContextAccessor">The HTTP context accessor.</param>
  public GetCollectionQueryHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
  {
    _context = context;
    _httpContextAccessor = httpContextAccessor;
  }

  /// <summary>
  /// Handles the query to get a collection by ID.
  /// </summary>
  /// <param name="request">The query.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The collection.</returns>
  public async Task<ApiResponse<CollectionDto>> Handle(GetCollectionQuery request, CancellationToken cancellationToken)
  {
    var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

    var collection = await _context.Collections
        .Include(c => c.MovieCollections)
        .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

    if (collection == null)
    {
      return ApiResponse<CollectionDto>.CreateError("Collection not found.");
    }

    // Check if the user has access to the collection
    if (!collection.IsPublic && collection.OwnerId != userId)
    {
      return ApiResponse<CollectionDto>.CreateError("You don't have permission to view this collection.");
    }

    var collectionDto = new CollectionDto
    {
      Id = collection.Id,
      Name = collection.Name,
      Description = collection.Description,
      IsPublic = collection.IsPublic,
      OwnerId = collection.OwnerId,
      CreatedOn = collection.CreatedOn,
      MovieCount = collection.MovieCollections.Count
    };

    return ApiResponse<CollectionDto>.CreateSuccess(collectionDto);
  }
}
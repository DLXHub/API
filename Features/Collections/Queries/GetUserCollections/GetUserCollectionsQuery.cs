using API.Features.Collections.Commands.CreateCollection;
using API.Shared.Infrastructure;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Collections.Queries.GetUserCollections;

/// <summary>
/// Query to get all collections for the current user.
/// </summary>
public record GetUserCollectionsQuery : IRequest<ApiResponse<List<CollectionDto>>>
{
}

/// <summary>
/// Handler for the GetUserCollectionsQuery.
/// </summary>
public class GetUserCollectionsQueryHandler : IRequestHandler<GetUserCollectionsQuery, ApiResponse<List<CollectionDto>>>
{
  private readonly ApplicationDbContext _context;
  private readonly IHttpContextAccessor _httpContextAccessor;

  /// <summary>
  /// Initializes a new instance of the GetUserCollectionsQueryHandler class.
  /// </summary>
  /// <param name="context">The database context.</param>
  /// <param name="httpContextAccessor">The HTTP context accessor.</param>
  public GetUserCollectionsQueryHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
  {
    _context = context;
    _httpContextAccessor = httpContextAccessor;
  }

  /// <summary>
  /// Handles the query to get all collections for the current user.
  /// </summary>
  /// <param name="request">The query.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of collections.</returns>
  public async Task<ApiResponse<List<CollectionDto>>> Handle(GetUserCollectionsQuery request, CancellationToken cancellationToken)
  {
    var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
    {
      return ApiResponse<List<CollectionDto>>.CreateError("User not authenticated.");
    }

    var collections = await _context.Collections
        .Where(c => c.OwnerId == userId)
        .Include(c => c.MovieCollections)
        .OrderByDescending(c => c.CreatedOn)
        .ToListAsync(cancellationToken);

    var collectionDtos = collections.Select(collection => new CollectionDto
    {
      Id = collection.Id,
      Name = collection.Name,
      Description = collection.Description,
      IsPublic = collection.IsPublic,
      OwnerId = collection.OwnerId,
      CreatedOn = collection.CreatedOn,
      MovieCount = collection.MovieCollections.Count
    }).ToList();

    return ApiResponse<List<CollectionDto>>.CreateSuccess(collectionDtos);
  }
}
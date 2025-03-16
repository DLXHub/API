using API.Shared.Infrastructure;
using API.Shared.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Collections.Queries.GetCollectionMovies;

/// <summary>
/// Query to get all movies in a collection.
/// </summary>
public record GetCollectionMoviesQuery : IRequest<ApiResponse<CollectionMoviesDto>>
{
  /// <summary>
  /// The ID of the collection.
  /// </summary>
  public Guid CollectionId { get; init; }
}

/// <summary>
/// DTO for a movie in a collection.
/// </summary>
public class MovieInCollectionDto
{
  /// <summary>
  /// The ID of the movie.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// The TMDB ID of the movie.
  /// </summary>
  public int TmdbId { get; set; }

  /// <summary>
  /// The title of the movie.
  /// </summary>
  public string Title { get; set; } = string.Empty;

  /// <summary>
  /// The original title of the movie.
  /// </summary>
  public string? OriginalTitle { get; set; }

  /// <summary>
  /// The overview or description of the movie.
  /// </summary>
  public string? Overview { get; set; }

  /// <summary>
  /// The poster path for the movie.
  /// </summary>
  public string? PosterPath { get; set; }

  /// <summary>
  /// The backdrop path for the movie.
  /// </summary>
  public string? BackdropPath { get; set; }

  /// <summary>
  /// The release date of the movie.
  /// </summary>
  public DateTime? ReleaseDate { get; set; }

  /// <summary>
  /// The order of the movie in the collection.
  /// </summary>
  public int Order { get; set; }

  /// <summary>
  /// Notes about the movie in this collection.
  /// </summary>
  public string? Notes { get; set; }

  /// <summary>
  /// The date when the movie was added to the collection.
  /// </summary>
  public DateTime AddedOn { get; set; }
}

/// <summary>
/// DTO for collection with movies.
/// </summary>
public class CollectionMoviesDto
{
  /// <summary>
  /// The ID of the collection.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// The name of the collection.
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// The description of the collection.
  /// </summary>
  public string? Description { get; set; }

  /// <summary>
  /// Indicates whether the collection is public or private.
  /// </summary>
  public bool IsPublic { get; set; }

  /// <summary>
  /// The ID of the owner.
  /// </summary>
  public string OwnerId { get; set; } = string.Empty;

  /// <summary>
  /// The movies in the collection.
  /// </summary>
  public List<MovieInCollectionDto> Movies { get; set; } = new List<MovieInCollectionDto>();
}

/// <summary>
/// Validator for the GetCollectionMoviesQuery.
/// </summary>
public class GetCollectionMoviesQueryValidator : AbstractValidator<GetCollectionMoviesQuery>
{
  /// <summary>
  /// Initializes a new instance of the GetCollectionMoviesQueryValidator class.
  /// </summary>
  public GetCollectionMoviesQueryValidator()
  {
    RuleFor(x => x.CollectionId)
        .NotEmpty().WithMessage("Collection ID is required.");
  }
}

/// <summary>
/// Handler for the GetCollectionMoviesQuery.
/// </summary>
public class GetCollectionMoviesQueryHandler : IRequestHandler<GetCollectionMoviesQuery, ApiResponse<CollectionMoviesDto>>
{
  private readonly ApplicationDbContext _context;
  private readonly IHttpContextAccessor _httpContextAccessor;

  /// <summary>
  /// Initializes a new instance of the GetCollectionMoviesQueryHandler class.
  /// </summary>
  /// <param name="context">The database context.</param>
  /// <param name="httpContextAccessor">The HTTP context accessor.</param>
  public GetCollectionMoviesQueryHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
  {
    _context = context;
    _httpContextAccessor = httpContextAccessor;
  }

  /// <summary>
  /// Handles the query to get all movies in a collection.
  /// </summary>
  /// <param name="request">The query.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The collection with its movies.</returns>
  public async Task<ApiResponse<CollectionMoviesDto>> Handle(GetCollectionMoviesQuery request, CancellationToken cancellationToken)
  {
    var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

    var collection = await _context.Collections
        .FirstOrDefaultAsync(c => c.Id == request.CollectionId, cancellationToken);

    if (collection == null)
    {
      return ApiResponse<CollectionMoviesDto>.CreateError("Collection not found.");
    }

    // Check if the user has access to the collection
    if (!collection.IsPublic && collection.OwnerId != userId)
    {
      return ApiResponse<CollectionMoviesDto>.CreateError("You don't have permission to view this collection.");
    }

    var movieCollections = await _context.MovieCollections
        .Where(mc => mc.CollectionId == request.CollectionId)
        .Include(mc => mc.Movie)
        .OrderBy(mc => mc.Order)
        .ToListAsync(cancellationToken);

    var collectionMoviesDto = new CollectionMoviesDto
    {
      Id = collection.Id,
      Name = collection.Name,
      Description = collection.Description,
      IsPublic = collection.IsPublic,
      OwnerId = collection.OwnerId,
      Movies = movieCollections.Select(mc => new MovieInCollectionDto
      {
        Id = mc.Movie.Id,
        TmdbId = mc.Movie.TmdbId,
        Title = mc.Movie.Title,
        OriginalTitle = mc.Movie.OriginalTitle,
        Overview = mc.Movie.Overview,
        PosterPath = mc.Movie.PosterPath,
        BackdropPath = mc.Movie.BackdropPath,
        ReleaseDate = mc.Movie.ReleaseDate,
        Order = mc.Order,
        Notes = mc.Notes,
        AddedOn = mc.AddedOn
      }).ToList()
    };

    return ApiResponse<CollectionMoviesDto>.CreateSuccess(collectionMoviesDto);
  }
}
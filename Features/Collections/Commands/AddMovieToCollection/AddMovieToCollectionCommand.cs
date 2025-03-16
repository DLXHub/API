using API.Features.Collections.Models;
using API.Features.Movies.Models;
using API.Shared.Infrastructure;
using API.Shared.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Collections.Commands.AddMovieToCollection;

/// <summary>
/// Command to add a movie to a collection.
/// </summary>
public record AddMovieToCollectionCommand : IRequest<ApiResponse<bool>>
{
  /// <summary>
  /// The ID of the collection to add the movie to.
  /// </summary>
  public Guid CollectionId { get; init; }

  /// <summary>
  /// The TMDB ID of the movie to add.
  /// </summary>
  public int TmdbId { get; init; }

  /// <summary>
  /// The title of the movie.
  /// </summary>
  public string Title { get; init; } = string.Empty;

  /// <summary>
  /// The original title of the movie.
  /// </summary>
  public string? OriginalTitle { get; init; }

  /// <summary>
  /// The overview or description of the movie.
  /// </summary>
  public string? Overview { get; init; }

  /// <summary>
  /// The poster path for the movie.
  /// </summary>
  public string? PosterPath { get; init; }

  /// <summary>
  /// The backdrop path for the movie.
  /// </summary>
  public string? BackdropPath { get; init; }

  /// <summary>
  /// The release date of the movie.
  /// </summary>
  public DateTime? ReleaseDate { get; init; }

  /// <summary>
  /// Notes about the movie in this collection.
  /// </summary>
  public string? Notes { get; init; }
}

/// <summary>
/// Validator for the AddMovieToCollectionCommand.
/// </summary>
public class AddMovieToCollectionCommandValidator : AbstractValidator<AddMovieToCollectionCommand>
{
  /// <summary>
  /// Initializes a new instance of the AddMovieToCollectionCommandValidator class.
  /// </summary>
  public AddMovieToCollectionCommandValidator()
  {
    RuleFor(x => x.CollectionId)
        .NotEmpty().WithMessage("Collection ID is required.");

    RuleFor(x => x.TmdbId)
        .NotEmpty().WithMessage("TMDB ID is required.");

    RuleFor(x => x.Title)
        .NotEmpty().WithMessage("Title is required.")
        .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

    RuleFor(x => x.Notes)
        .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.");
  }
}

/// <summary>
/// Handler for the AddMovieToCollectionCommand.
/// </summary>
public class AddMovieToCollectionCommandHandler : IRequestHandler<AddMovieToCollectionCommand, ApiResponse<bool>>
{
  private readonly ApplicationDbContext _context;
  private readonly IHttpContextAccessor _httpContextAccessor;

  /// <summary>
  /// Initializes a new instance of the AddMovieToCollectionCommandHandler class.
  /// </summary>
  /// <param name="context">The database context.</param>
  /// <param name="httpContextAccessor">The HTTP context accessor.</param>
  public AddMovieToCollectionCommandHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
  {
    _context = context;
    _httpContextAccessor = httpContextAccessor;
  }

  /// <summary>
  /// Handles the command to add a movie to a collection.
  /// </summary>
  /// <param name="request">The command.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>True if the movie was added successfully.</returns>
  public async Task<ApiResponse<bool>> Handle(AddMovieToCollectionCommand request, CancellationToken cancellationToken)
  {
    var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
    {
      return ApiResponse<bool>.CreateError("User not authenticated.");
    }

    // Check if the collection exists and belongs to the user
    var collection = await _context.Collections
        .FirstOrDefaultAsync(c => c.Id == request.CollectionId, cancellationToken);

    if (collection == null)
    {
      return ApiResponse<bool>.CreateError("Collection not found.");
    }

    if (collection.OwnerId != userId)
    {
      return ApiResponse<bool>.CreateError("You don't have permission to modify this collection.");
    }

    // Check if the movie already exists in the database
    var movie = await _context.Movies
        .FirstOrDefaultAsync(m => m.TmdbId == request.TmdbId, cancellationToken);

    // If the movie doesn't exist, create it
    if (movie == null)
    {
      movie = new Movie
      {
        TmdbId = request.TmdbId,
        Title = request.Title,
        OriginalTitle = request.OriginalTitle,
        Overview = request.Overview,
        PosterPath = request.PosterPath,
        BackdropPath = request.BackdropPath,
        ReleaseDate = request.ReleaseDate
      };

      _context.Movies.Add(movie);
    }

    // Check if the movie is already in the collection
    var existingMovieCollection = await _context.MovieCollections
        .FirstOrDefaultAsync(mc => mc.CollectionId == request.CollectionId && mc.Movie.TmdbId == request.TmdbId, cancellationToken);

    if (existingMovieCollection != null)
    {
      return ApiResponse<bool>.CreateError("This movie is already in the collection.");
    }

    // Get the current highest order in the collection
    var highestOrder = await _context.MovieCollections
        .Where(mc => mc.CollectionId == request.CollectionId)
        .OrderByDescending(mc => mc.Order)
        .Select(mc => mc.Order)
        .FirstOrDefaultAsync(cancellationToken);

    // Add the movie to the collection
    var movieCollection = new MovieCollection
    {
      CollectionId = request.CollectionId,
      Movie = movie,
      Order = highestOrder + 1,
      Notes = request.Notes
    };

    _context.MovieCollections.Add(movieCollection);
    await _context.SaveChangesAsync(cancellationToken);

    return ApiResponse<bool>.CreateSuccess(true, "Movie added to collection successfully.");
  }
}
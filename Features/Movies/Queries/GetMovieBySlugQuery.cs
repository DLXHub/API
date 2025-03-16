using API.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Movies.Queries;

/// <summary>
/// Query to get a movie by its slug
/// </summary>
public class GetMovieBySlugQuery : IRequest<MovieDto?>
{
  /// <summary>
  /// The slug of the movie to retrieve
  /// </summary>
  public string Slug { get; set; } = string.Empty;
}

/// <summary>
/// Handler for the GetMovieBySlugQuery
/// </summary>
public class GetMovieBySlugQueryHandler : IRequestHandler<GetMovieBySlugQuery, MovieDto?>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetMovieBySlugQueryHandler class
  /// </summary>
  /// <param name="context">The database context</param>
  public GetMovieBySlugQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the GetMovieBySlugQuery
  /// </summary>
  /// <param name="request">The query</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The movie DTO or null if not found</returns>
  public async Task<MovieDto?> Handle(GetMovieBySlugQuery request, CancellationToken cancellationToken)
  {
    var movie = await _context.Movies
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.Slug == request.Slug, cancellationToken);

    if (movie == null)
    {
      return null;
    }

    return new MovieDto
    {
      Id = movie.Id,
      Title = movie.Title,
      OriginalTitle = movie.OriginalTitle,
      Overview = movie.Overview,
      PosterPath = movie.PosterPath,
      BackdropPath = movie.BackdropPath,
      ReleaseDate = movie.ReleaseDate,
      Runtime = movie.Runtime,
      VoteAverage = movie.VoteAverage,
      TmdbId = movie.TmdbId,
      Slug = movie.Slug
    };
  }
}
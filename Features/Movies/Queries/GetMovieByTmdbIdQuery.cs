using System.Threading;
using System.Threading.Tasks;
using API.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Movies.Queries;

/// <summary>
/// Query to get a movie by TMDB ID
/// </summary>
public class GetMovieByTmdbIdQuery : IRequest<MovieDto?>
{
  /// <summary>
  /// The TMDB ID of the movie to retrieve
  /// </summary>
  public int TmdbId { get; set; }
}

/// <summary>
/// Handler for the GetMovieByTmdbIdQuery
/// </summary>
public class GetMovieByTmdbIdQueryHandler : IRequestHandler<GetMovieByTmdbIdQuery, MovieDto?>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetMovieByTmdbIdQueryHandler class
  /// </summary>
  /// <param name="context">The database context</param>
  public GetMovieByTmdbIdQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the GetMovieByTmdbIdQuery
  /// </summary>
  /// <param name="request">The query</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The movie DTO or null if not found</returns>
  public async Task<MovieDto?> Handle(GetMovieByTmdbIdQuery request, CancellationToken cancellationToken)
  {
    var movie = await _context.Movies
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.TmdbId == request.TmdbId, cancellationToken);

    if (movie == null)
    {
      return null;
    }

    return MovieDto.FromEntity(movie);
  }
}
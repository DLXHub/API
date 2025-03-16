using System;
using System.Threading;
using System.Threading.Tasks;
using API.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Movies.Queries;

/// <summary>
/// Query to get a movie by ID
/// </summary>
public class GetMovieQuery : IRequest<MovieDto?>
{
  /// <summary>
  /// The ID of the movie to retrieve
  /// </summary>
  public Guid Id { get; set; }
}

/// <summary>
/// Handler for the GetMovieQuery
/// </summary>
public class GetMovieQueryHandler : IRequestHandler<GetMovieQuery, MovieDto?>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetMovieQueryHandler class
  /// </summary>
  /// <param name="context">The database context</param>
  public GetMovieQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the GetMovieQuery
  /// </summary>
  /// <param name="request">The query</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The movie DTO or null if not found</returns>
  public async Task<MovieDto?> Handle(GetMovieQuery request, CancellationToken cancellationToken)
  {
    var movie = await _context.Movies
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

    if (movie == null)
    {
      return null;
    }

    return MovieDto.FromEntity(movie);
  }
}
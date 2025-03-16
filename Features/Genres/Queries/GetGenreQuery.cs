using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Features.Movies.Models;
using API.Features.TvShows.Models;
using API.Shared.Infrastructure;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Genres.Queries;

/// <summary>
/// Query to get a genre by ID
/// </summary>
public class GetGenreQuery : IRequest<GenreDto?>
{
  /// <summary>
  /// The ID of the genre to retrieve
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Filter genres by media type. If specified, only returns the genre if it's used by the specified media type.
  /// Possible values: "movie", "tv", or null (no filter)
  /// </summary>
  public string? MediaType { get; set; }
}

/// <summary>
/// Handler for the GetGenreQuery
/// </summary>
public class GetGenreQueryHandler : IRequestHandler<GetGenreQuery, GenreDto?>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetGenreQueryHandler class
  /// </summary>
  /// <param name="context">The database context</param>
  public GetGenreQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the GetGenreQuery
  /// </summary>
  /// <param name="request">The query</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The genre DTO or null if not found</returns>
  public async Task<GenreDto?> Handle(GetGenreQuery request, CancellationToken cancellationToken)
  {
    var query = _context.Genres
        .AsNoTracking()
        .Where(g => g.Id == request.Id);

    // Apply media type filter if specified
    if (!string.IsNullOrWhiteSpace(request.MediaType))
    {
      if (request.MediaType.ToLower() == "movie")
      {
        // Filter for genres used by movies
        query = query.Where(g => g.MediaGenres.Any(mg =>
          _context.Movies.Any(m => m.Id == mg.MediaId)));
      }
      else if (request.MediaType.ToLower() == "tv")
      {
        // Filter for genres used by TV shows
        query = query.Where(g => g.MediaGenres.Any(mg =>
          _context.TvShows.Any(t => t.Id == mg.MediaId)));
      }
    }

    var genre = await query.FirstOrDefaultAsync(cancellationToken);

    if (genre == null)
    {
      return null;
    }

    return GenreDto.FromEntity(genre);
  }
}
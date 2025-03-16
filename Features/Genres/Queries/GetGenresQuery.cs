using System.Collections.Generic;
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
/// Query to get all genres
/// </summary>
public class GetGenresQuery : IRequest<List<GenreDto>>
{
  /// <summary>
  /// Whether to sort by name (true) or ID (false)
  /// </summary>
  public bool SortByName { get; set; } = true;

  /// <summary>
  /// Filter genres by media type. If specified, only returns genres used by the specified media type.
  /// Possible values: "movie", "tv", or null (no filter)
  /// </summary>
  public string? MediaType { get; set; }
}

/// <summary>
/// Handler for the GetGenresQuery
/// </summary>
public class GetGenresQueryHandler : IRequestHandler<GetGenresQuery, List<GenreDto>>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetGenresQueryHandler class
  /// </summary>
  /// <param name="context">The database context</param>
  public GetGenresQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the GetGenresQuery
  /// </summary>
  /// <param name="request">The query</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>A list of all genres</returns>
  public async Task<List<GenreDto>> Handle(GetGenresQuery request, CancellationToken cancellationToken)
  {
    var query = _context.Genres.AsNoTracking();

    // Apply media type filter if specified
    if (!string.IsNullOrWhiteSpace(request.MediaType))
    {
      if (request.MediaType.ToLower() == "movie")
      {
        query = query.Where(g => g.MediaGenres.Any(mg =>
          _context.Movies.Any(m => m.Id == mg.MediaId)));
      }
      else if (request.MediaType.ToLower() == "tv")
      {
        query = query.Where(g => g.MediaGenres.Any(mg =>
          _context.TvShows.Any(t => t.Id == mg.MediaId)));
      }
    }

    // Apply sorting
    query = request.SortByName
        ? query.OrderBy(g => g.Name)
        : query.OrderBy(g => g.TmdbId);

    var genres = await query.ToListAsync(cancellationToken);

    return genres.Select(GenreDto.FromEntity).ToList();
  }
}
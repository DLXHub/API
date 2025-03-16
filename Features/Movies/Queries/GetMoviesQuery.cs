using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Shared.Infrastructure;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Movies.Queries;

/// <summary>
/// Query to get a paginated list of movies
/// </summary>
public class GetMoviesQuery : IRequest<PaginatedList<MovieListItemDto>>
{
  /// <summary>
  /// The page number (1-based)
  /// </summary>
  public int PageNumber { get; set; } = 1;

  /// <summary>
  /// The number of items per page
  /// </summary>
  public int PageSize { get; set; } = 20;

  /// <summary>
  /// Optional search term to filter movies by title
  /// </summary>
  public string? SearchTerm { get; set; }

  /// <summary>
  /// Optional filter for release year
  /// </summary>
  public int? ReleaseYear { get; set; }

  /// <summary>
  /// Whether to sort by popularity (true) or title (false)
  /// </summary>
  public bool SortByPopularity { get; set; } = true;

  /// <summary>
  /// Whether to sort in descending order
  /// </summary>
  public bool SortDescending { get; set; } = true;
}

/// <summary>
/// Handler for the GetMoviesQuery
/// </summary>
public class GetMoviesQueryHandler : IRequestHandler<GetMoviesQuery, PaginatedList<MovieListItemDto>>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetMoviesQueryHandler class
  /// </summary>
  /// <param name="context">The database context</param>
  public GetMoviesQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the GetMoviesQuery
  /// </summary>
  /// <param name="request">The query</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>A paginated list of movies</returns>
  public async Task<PaginatedList<MovieListItemDto>> Handle(GetMoviesQuery request, CancellationToken cancellationToken)
  {
    var query = _context.Movies.AsNoTracking();

    // Apply search filter
    if (!string.IsNullOrWhiteSpace(request.SearchTerm))
    {
      var searchTerm = request.SearchTerm.ToLower();
      query = query.Where(m => m.Title.ToLower().Contains(searchTerm) ||
                              (m.OriginalTitle != null && m.OriginalTitle.ToLower().Contains(searchTerm)));
    }

    // Apply release year filter
    if (request.ReleaseYear.HasValue)
    {
      query = query.Where(m => m.ReleaseDate != null &&
                              m.ReleaseDate.Value.Year == request.ReleaseYear.Value);
    }

    // Apply sorting
    if (request.SortByPopularity)
    {
      query = request.SortDescending
          ? query.OrderByDescending(m => m.Popularity)
          : query.OrderBy(m => m.Popularity);
    }
    else
    {
      query = request.SortDescending
          ? query.OrderByDescending(m => m.Title)
          : query.OrderBy(m => m.Title);
    }

    // Map to DTOs and paginate
    var dtoQuery = query.Select(m => MovieListItemDto.FromEntity(m));

    return await PaginatedList<MovieListItemDto>.CreateAsync(
        dtoQuery, request.PageNumber, request.PageSize);
  }
}
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Shared.Infrastructure;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.People.Queries;

/// <summary>
/// Query to get a list of people with optional filtering and pagination
/// </summary>
public class GetPeopleQuery : IRequest<PaginatedList<PersonListItemDto>>
{
  /// <summary>
  /// The search term to filter people by name
  /// </summary>
  public string? SearchTerm { get; set; }

  /// <summary>
  /// The page number (1-based)
  /// </summary>
  public int PageNumber { get; set; } = 1;

  /// <summary>
  /// The page size
  /// </summary>
  public int PageSize { get; set; } = 20;

  /// <summary>
  /// The department to filter by (e.g., "Acting", "Directing")
  /// </summary>
  public string? Department { get; set; }

  /// <summary>
  /// Whether to sort by popularity (descending)
  /// </summary>
  public bool SortByPopularity { get; set; } = true;
}

/// <summary>
/// Handler for the GetPeopleQuery
/// </summary>
public class GetPeopleQueryHandler : IRequestHandler<GetPeopleQuery, PaginatedList<PersonListItemDto>>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetPeopleQueryHandler class
  /// </summary>
  /// <param name="context">The database context</param>
  public GetPeopleQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the GetPeopleQuery
  /// </summary>
  /// <param name="request">The query</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>A paginated list of people</returns>
  public async Task<PaginatedList<PersonListItemDto>> Handle(GetPeopleQuery request, CancellationToken cancellationToken)
  {
    var query = _context.People.AsNoTracking();

    // Apply search filter
    if (!string.IsNullOrWhiteSpace(request.SearchTerm))
    {
      var searchTerm = request.SearchTerm.ToLower();
      query = query.Where(p => p.Name.ToLower().Contains(searchTerm));
    }

    // Apply department filter
    if (!string.IsNullOrWhiteSpace(request.Department))
    {
      query = query.Where(p => p.KnownForDepartment == request.Department);
    }

    // Apply sorting
    if (request.SortByPopularity)
    {
      query = query.OrderByDescending(p => p.Popularity);
    }
    else
    {
      query = query.OrderBy(p => p.Name);
    }

    // Map to DTOs and paginate
    var dtoQuery = query.Select(p => PersonListItemDto.FromEntity(p));

    return await PaginatedList<PersonListItemDto>.CreateAsync(
        dtoQuery,
        request.PageNumber,
        request.PageSize);
  }
}
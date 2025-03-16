using API.Features.Pages.Models;
using API.Shared.Infrastructure;
using API.Shared.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Pages.Queries.GetPages;

/// <summary>
/// Query to get a paginated list of pages.
/// </summary>
public record GetPagesQuery : IRequest<ApiResponse<PaginatedList<PageDto>>>
{
  /// <summary>
  /// The page number to retrieve (1-based).
  /// </summary>
  public int PageNumber { get; init; } = 1;

  /// <summary>
  /// The number of items per page.
  /// </summary>
  public int PageSize { get; init; } = 10;

  /// <summary>
  /// Optional search term to filter pages by title.
  /// </summary>
  public string? SearchTerm { get; init; }

  /// <summary>
  /// Optional status filter.
  /// </summary>
  public PageStatus? Status { get; init; }

  /// <summary>
  /// Whether to include draft versions.
  /// </summary>
  public bool IncludeDrafts { get; init; }
}

/// <summary>
/// Validator for the GetPagesQuery.
/// </summary>
public class GetPagesQueryValidator : AbstractValidator<GetPagesQuery>
{
  /// <summary>
  /// Initializes a new instance of the GetPagesQueryValidator class.
  /// </summary>
  public GetPagesQueryValidator()
  {
    RuleFor(x => x.PageNumber)
        .GreaterThan(0);

    RuleFor(x => x.PageSize)
        .GreaterThan(0)
        .LessThanOrEqualTo(100);

    RuleFor(x => x.SearchTerm)
        .MaximumLength(255)
        .When(x => x.SearchTerm != null);
  }
}

/// <summary>
/// Handler for the GetPagesQuery.
/// </summary>
public class GetPagesQueryHandler : IRequestHandler<GetPagesQuery, ApiResponse<PaginatedList<PageDto>>>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetPagesQueryHandler class.
  /// </summary>
  public GetPagesQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the GetPagesQuery.
  /// </summary>
  public async Task<ApiResponse<PaginatedList<PageDto>>> Handle(GetPagesQuery request, CancellationToken cancellationToken)
  {
    var query = _context.Pages
        .AsNoTracking()
        .Include(p => p.CreatedBy)
        .Include(p => p.ModifiedBy)
        .Include(p => p.PublishedBy)
        .AsQueryable();

    // Apply filters
    if (!request.IncludeDrafts)
    {
      query = query.Where(p => p.Status == PageStatus.Published);
    }

    if (request.Status.HasValue)
    {
      query = query.Where(p => p.Status == request.Status.Value);
    }

    if (!string.IsNullOrWhiteSpace(request.SearchTerm))
    {
      var searchTerm = request.SearchTerm.ToLower();
      query = query.Where(p =>
          p.Title.ToLower().Contains(searchTerm) ||
          (p.SeoTitle != null && p.SeoTitle.ToLower().Contains(searchTerm)) ||
          p.Slug.ToLower().Contains(searchTerm) ||
          p.LinkTarget.ToLower().Contains(searchTerm));
    }

    // Order by status (published first) and then by title
    query = query.OrderBy(p => p.Status != PageStatus.Published)
                .ThenBy(p => p.Title);

    var pages = await PaginatedList<Page>.CreateAsync(
        query,
        request.PageNumber,
        request.PageSize);

    var paginatedDtos = new PaginatedList<PageDto>(
        pages.Items.Select(PageDto.FromEntity).ToList(),
        pages.TotalCount,
        pages.PageNumber,
        pages.PageSize);

    return ApiResponse<PaginatedList<PageDto>>.CreateSuccess(paginatedDtos);
  }
}
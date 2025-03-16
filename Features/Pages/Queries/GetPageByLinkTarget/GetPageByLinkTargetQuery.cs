using System.Text.Json;
using API.Features.Pages.Models;
using API.Shared.Infrastructure;
using API.Shared.Models;
using Ardalis.GuardClauses;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace API.Features.Pages.Queries.GetPageByLinkTarget;

/// <summary>
/// Query to get a page by its link target.
/// </summary>
public record GetPageByLinkTargetQuery : IRequest<ApiResponse<PageDto>>
{
  /// <summary>
  /// The link target of the page to retrieve.
  /// </summary>
  public string LinkTarget { get; init; } = string.Empty;

  /// <summary>
  /// Whether to include draft versions in the search.
  /// </summary>
  public bool IncludeDrafts { get; init; }
}

/// <summary>
/// Validator for the GetPageByLinkTargetQuery.
/// </summary>
public class GetPageByLinkTargetQueryValidator : AbstractValidator<GetPageByLinkTargetQuery>
{
  /// <summary>
  /// Initializes a new instance of the GetPageByLinkTargetQueryValidator class.
  /// </summary>
  public GetPageByLinkTargetQueryValidator()
  {
    RuleFor(x => x.LinkTarget)
        .NotEmpty()
        .MaximumLength(100)
        .Matches("^[a-zA-Z0-9_]+$")
        .WithMessage("Invalid link target format.");
  }
}

/// <summary>
/// Handler for the GetPageByLinkTargetQuery.
/// </summary>
public class GetPageByLinkTargetQueryHandler : IRequestHandler<GetPageByLinkTargetQuery, ApiResponse<PageDto>>
{
  private readonly ApplicationDbContext _context;
  private readonly IDistributedCache _cache;
  private const string CacheKeyPattern = "page:linktarget:{0}";

  /// <summary>
  /// Initializes a new instance of the GetPageByLinkTargetQueryHandler class.
  /// </summary>
  public GetPageByLinkTargetQueryHandler(ApplicationDbContext context, IDistributedCache cache)
  {
    _context = context;
    _cache = cache;
  }

  /// <summary>
  /// Handles the GetPageByLinkTargetQuery.
  /// </summary>
  public async Task<ApiResponse<PageDto>> Handle(GetPageByLinkTargetQuery request, CancellationToken cancellationToken)
  {
    // Only use cache for published pages
    if (!request.IncludeDrafts)
    {
      var cacheKey = string.Format(CacheKeyPattern, request.LinkTarget);
      var cachedPage = await GetFromCache(cacheKey);
      if (cachedPage != null)
      {
        return ApiResponse<PageDto>.CreateSuccess(cachedPage);
      }
    }

    var query = _context.Pages.AsNoTracking();

    if (!request.IncludeDrafts)
    {
      query = query.Where(p => p.Status == PageStatus.Published);
    }

    var page = await query
        .Include(p => p.CreatedBy)
        .Include(p => p.ModifiedBy)
        .Include(p => p.PublishedBy)
        .FirstOrDefaultAsync(p => p.LinkTarget == request.LinkTarget, cancellationToken);

    Guard.Against.NotFound(request.LinkTarget, page);

    var pageDto = PageDto.FromEntity(page);

    // Cache only published pages
    if (page.Status == PageStatus.Published)
    {
      var cacheKey = string.Format(CacheKeyPattern, request.LinkTarget);
      await CachePageDto(cacheKey, pageDto);
    }

    return ApiResponse<PageDto>.CreateSuccess(pageDto);
  }

  private async Task<PageDto?> GetFromCache(string cacheKey)
  {
    var cachedValue = await _cache.GetStringAsync(cacheKey);
    return cachedValue == null ? null : JsonSerializer.Deserialize<PageDto>(cachedValue);
  }

  private async Task CachePageDto(string cacheKey, PageDto pageDto)
  {
    var options = new DistributedCacheEntryOptions
    {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
    };

    await _cache.SetStringAsync(
        cacheKey,
        JsonSerializer.Serialize(pageDto),
        options);
  }
}
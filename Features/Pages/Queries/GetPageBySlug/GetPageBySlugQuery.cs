using System.Text.Json;
using API.Features.Pages.Models;
using API.Shared.Infrastructure;
using API.Shared.Models;
using Ardalis.GuardClauses;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace API.Features.Pages.Queries.GetPageBySlug;

/// <summary>
/// Query to get a page by its slug.
/// </summary>
public record GetPageBySlugQuery : IRequest<ApiResponse<PageDto>>
{
  /// <summary>
  /// The slug of the page to retrieve.
  /// </summary>
  public string Slug { get; init; } = string.Empty;

  /// <summary>
  /// Whether to include draft versions in the search.
  /// </summary>
  public bool IncludeDrafts { get; init; }
}

/// <summary>
/// Validator for the GetPageBySlugQuery.
/// </summary>
public class GetPageBySlugQueryValidator : AbstractValidator<GetPageBySlugQuery>
{
  /// <summary>
  /// Initializes a new instance of the GetPageBySlugQueryValidator class.
  /// </summary>
  public GetPageBySlugQueryValidator()
  {
    RuleFor(x => x.Slug)
        .NotEmpty()
        .MaximumLength(255)
        .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
        .WithMessage("Invalid slug format.");
  }
}

/// <summary>
/// Handler for the GetPageBySlugQuery.
/// </summary>
public class GetPageBySlugQueryHandler : IRequestHandler<GetPageBySlugQuery, ApiResponse<PageDto>>
{
  private readonly ApplicationDbContext _context;
  private readonly IDistributedCache _cache;
  private const string CacheKeyPattern = "page:slug:{0}";

  /// <summary>
  /// Initializes a new instance of the GetPageBySlugQueryHandler class.
  /// </summary>
  public GetPageBySlugQueryHandler(ApplicationDbContext context, IDistributedCache cache)
  {
    _context = context;
    _cache = cache;
  }

  /// <summary>
  /// Handles the GetPageBySlugQuery.
  /// </summary>
  public async Task<ApiResponse<PageDto>> Handle(GetPageBySlugQuery request, CancellationToken cancellationToken)
  {
    // Only use cache for published pages
    if (!request.IncludeDrafts)
    {
      var cacheKey = string.Format(CacheKeyPattern, request.Slug);
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
        .FirstOrDefaultAsync(p => p.Slug == request.Slug, cancellationToken);

    Guard.Against.NotFound(request.Slug, page);

    var pageDto = PageDto.FromEntity(page);

    // Cache only published pages
    if (page.Status == PageStatus.Published)
    {
      var cacheKey = string.Format(CacheKeyPattern, request.Slug);
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
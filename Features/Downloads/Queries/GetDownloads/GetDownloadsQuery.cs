using API.Features.Downloads.Models;
using API.Shared.Infrastructure;
using API.Shared.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Downloads.Queries.GetDownloads;

/// <summary>
/// Query to get downloads with filtering options.
/// </summary>
public record GetDownloadsQuery : IRequest<ApiResponse<PaginatedList<DownloadDto>>>
{
  /// <summary>
  /// The type of media to filter by.
  /// </summary>
  public MediaType? MediaType { get; init; }

  /// <summary>
  /// The ID of the specific media item to get downloads for.
  /// </summary>
  public Guid? MediaId { get; init; }

  /// <summary>
  /// The language to filter by.
  /// </summary>
  public string? Language { get; init; }

  /// <summary>
  /// The quality to filter by.
  /// </summary>
  public string? Quality { get; init; }

  /// <summary>
  /// The page number (1-based).
  /// </summary>
  public int PageNumber { get; init; } = 1;

  /// <summary>
  /// The number of items per page.
  /// </summary>
  public int PageSize { get; init; } = 10;
}

/// <summary>
/// Validator for the GetDownloadsQuery.
/// </summary>
public class GetDownloadsQueryValidator : AbstractValidator<GetDownloadsQuery>
{
  /// <summary>
  /// Initializes a new instance of the GetDownloadsQueryValidator class.
  /// </summary>
  public GetDownloadsQueryValidator()
  {
    RuleFor(x => x.PageNumber)
        .GreaterThan(0).WithMessage("Page number must be greater than 0.");

    RuleFor(x => x.PageSize)
        .GreaterThan(0).WithMessage("Page size must be greater than 0.")
        .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.");

    When(x => x.MediaId.HasValue, () =>
    {
      RuleFor(x => x.MediaType)
              .NotNull().WithMessage("Media type must be specified when media ID is provided.");
    });
  }
}

/// <summary>
/// Handler for the GetDownloadsQuery.
/// </summary>
public class GetDownloadsQueryHandler : IRequestHandler<GetDownloadsQuery, ApiResponse<PaginatedList<DownloadDto>>>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetDownloadsQueryHandler class.
  /// </summary>
  /// <param name="context">The database context.</param>
  public GetDownloadsQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the query to get downloads.
  /// </summary>
  /// <param name="request">The query.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>A paginated list of downloads.</returns>
  public async Task<ApiResponse<PaginatedList<DownloadDto>>> Handle(GetDownloadsQuery request, CancellationToken cancellationToken)
  {
    var query = _context.Downloads
        .Include(d => d.Movie)
        .Include(d => d.TvShow)
        .Include(d => d.Season).ThenInclude(s => s!.TvShow)
        .Include(d => d.Episode).ThenInclude(e => e!.Season).ThenInclude(s => s!.TvShow)
        .AsNoTracking();

    // Apply filters
    if (request.MediaType.HasValue)
    {
      query = query.Where(d => d.MediaType == request.MediaType.Value);

      if (request.MediaId.HasValue)
      {
        query = request.MediaType.Value switch
        {
          MediaType.Movie => query.Where(d => d.MovieId == request.MediaId.Value),
          MediaType.TvShow => query.Where(d => d.TvShowId == request.MediaId.Value),
          MediaType.Season => query.Where(d => d.SeasonId == request.MediaId.Value),
          MediaType.Episode => query.Where(d => d.EpisodeId == request.MediaId.Value),
          _ => query
        };
      }
    }

    if (!string.IsNullOrWhiteSpace(request.Language))
    {
      query = query.Where(d => d.Language == request.Language);
    }

    if (!string.IsNullOrWhiteSpace(request.Quality))
    {
      query = query.Where(d => d.Quality == request.Quality);
    }

    // Order by creation date (newest first)
    query = query.OrderByDescending(d => d.CreatedOn);

    // Create paginated result
    var paginatedList = await PaginatedList<Download>.CreateAsync(
        query, request.PageNumber, request.PageSize);

    // Map to DTOs
    var downloadDtos = paginatedList.Items.Select(DownloadDto.FromEntity).ToList();

    var result = new PaginatedList<DownloadDto>(
        downloadDtos,
        paginatedList.TotalCount,
        paginatedList.PageNumber,
        paginatedList.PageSize);

    return ApiResponse<PaginatedList<DownloadDto>>.CreateSuccess(result);
  }
}
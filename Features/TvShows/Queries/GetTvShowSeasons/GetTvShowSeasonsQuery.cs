using API.Shared.Infrastructure;
using API.Shared.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.TvShows.Queries.GetTvShowSeasons;

/// <summary>
/// DTO for a season.
/// </summary>
public class SeasonDto
{
  /// <summary>
  /// The ID of the season.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// The TMDB ID of the season.
  /// </summary>
  public int TmdbId { get; set; }

  /// <summary>
  /// The season number.
  /// </summary>
  public int SeasonNumber { get; set; }

  /// <summary>
  /// The name of the season.
  /// </summary>
  public string? Name { get; set; }

  /// <summary>
  /// The overview or description of the season.
  /// </summary>
  public string? Overview { get; set; }

  /// <summary>
  /// The poster path for the season.
  /// </summary>
  public string? PosterPath { get; set; }

  /// <summary>
  /// The air date of the season.
  /// </summary>
  public DateTime? AirDate { get; set; }

  /// <summary>
  /// The number of episodes in the season.
  /// </summary>
  public int? EpisodeCount { get; set; }
}

/// <summary>
/// Query to get all seasons of a TV show.
/// </summary>
public record GetTvShowSeasonsQuery : IRequest<ApiResponse<List<SeasonDto>>>
{
  /// <summary>
  /// The ID of the TV show.
  /// </summary>
  public Guid TvShowId { get; init; }
}

/// <summary>
/// Validator for the GetTvShowSeasonsQuery.
/// </summary>
public class GetTvShowSeasonsQueryValidator : AbstractValidator<GetTvShowSeasonsQuery>
{
  /// <summary>
  /// Initializes a new instance of the GetTvShowSeasonsQueryValidator class.
  /// </summary>
  public GetTvShowSeasonsQueryValidator()
  {
    RuleFor(x => x.TvShowId)
        .NotEmpty().WithMessage("TV show ID is required.");
  }
}

/// <summary>
/// Handler for the GetTvShowSeasonsQuery.
/// </summary>
public class GetTvShowSeasonsQueryHandler : IRequestHandler<GetTvShowSeasonsQuery, ApiResponse<List<SeasonDto>>>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetTvShowSeasonsQueryHandler class.
  /// </summary>
  /// <param name="context">The database context.</param>
  public GetTvShowSeasonsQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the query to get all seasons of a TV show.
  /// </summary>
  /// <param name="request">The query.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of seasons.</returns>
  public async Task<ApiResponse<List<SeasonDto>>> Handle(GetTvShowSeasonsQuery request, CancellationToken cancellationToken)
  {
    // Check if the TV show exists
    var tvShowExists = await _context.TvShows
        .AnyAsync(t => t.Id == request.TvShowId, cancellationToken);

    if (!tvShowExists)
    {
      return ApiResponse<List<SeasonDto>>.CreateError("TV show not found.");
    }

    var seasons = await _context.Seasons
        .Where(s => s.TvShowId == request.TvShowId)
        .OrderBy(s => s.SeasonNumber)
        .ToListAsync(cancellationToken);

    var seasonDtos = seasons.Select(season => new SeasonDto
    {
      Id = season.Id,
      TmdbId = season.TmdbId,
      SeasonNumber = season.SeasonNumber,
      Name = season.Name,
      Overview = season.Overview,
      PosterPath = season.PosterPath,
      AirDate = season.AirDate,
      EpisodeCount = season.EpisodeCount
    }).ToList();

    return ApiResponse<List<SeasonDto>>.CreateSuccess(seasonDtos);
  }
}
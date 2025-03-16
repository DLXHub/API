using API.Features.TvShows.Commands.ImportTvShow;
using API.Shared.Infrastructure;
using API.Shared.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.TvShows.Queries.GetTvShowsBySeason;

/// <summary>
/// Query to get TV shows filtered by season number.
/// </summary>
public record GetTvShowsBySeasonQuery : IRequest<ApiResponse<List<TvShowDto>>>
{
  /// <summary>
  /// The season number to filter by. If provided, only returns TV shows that have this season.
  /// </summary>
  public int? SeasonNumber { get; init; }

  /// <summary>
  /// Whether to include seasons with the specified number only (true) or all seasons up to and including the specified number (false).
  /// </summary>
  public bool ExactMatch { get; init; } = true;

  /// <summary>
  /// Whether to include TV shows that have episodes in the specified season (true) or just having the season is enough (false).
  /// </summary>
  public bool HasEpisodes { get; init; } = false;

  /// <summary>
  /// The minimum number of episodes a season should have to be included (only used when HasEpisodes is true).
  /// </summary>
  public int? MinEpisodeCount { get; init; }
}

/// <summary>
/// Validator for the GetTvShowsBySeasonQuery.
/// </summary>
public class GetTvShowsBySeasonQueryValidator : AbstractValidator<GetTvShowsBySeasonQuery>
{
  /// <summary>
  /// Initializes a new instance of the GetTvShowsBySeasonQueryValidator class.
  /// </summary>
  public GetTvShowsBySeasonQueryValidator()
  {
    RuleFor(x => x.SeasonNumber)
        .GreaterThanOrEqualTo(0).When(x => x.SeasonNumber.HasValue)
        .WithMessage("Season number must be non-negative.");

    RuleFor(x => x.MinEpisodeCount)
        .GreaterThanOrEqualTo(1).When(x => x.MinEpisodeCount.HasValue)
        .WithMessage("Minimum episode count must be at least 1.");
  }
}

/// <summary>
/// Handler for the GetTvShowsBySeasonQuery.
/// </summary>
public class GetTvShowsBySeasonQueryHandler : IRequestHandler<GetTvShowsBySeasonQuery, ApiResponse<List<TvShowDto>>>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetTvShowsBySeasonQueryHandler class.
  /// </summary>
  /// <param name="context">The database context.</param>
  public GetTvShowsBySeasonQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the query to get TV shows filtered by season number.
  /// </summary>
  /// <param name="request">The query.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of TV shows.</returns>
  public async Task<ApiResponse<List<TvShowDto>>> Handle(GetTvShowsBySeasonQuery request, CancellationToken cancellationToken)
  {
    var query = _context.TvShows.AsNoTracking();

    // Apply season number filter if specified
    if (request.SeasonNumber.HasValue)
    {
      if (request.ExactMatch)
      {
        // Exact match: TV shows that have exactly this season number
        query = query.Where(t => t.Seasons.Any(s => s.SeasonNumber == request.SeasonNumber.Value));
      }
      else
      {
        // TV shows that have at least this many seasons
        query = query.Where(t => t.Seasons.Any(s => s.SeasonNumber >= request.SeasonNumber.Value));
      }

      // Additional filter for episodes
      if (request.HasEpisodes)
      {
        if (request.MinEpisodeCount.HasValue)
        {
          // TV shows with seasons that have at least the minimum number of episodes
          query = query.Where(t => t.Seasons
              .Where(s => request.ExactMatch ? s.SeasonNumber == request.SeasonNumber.Value : s.SeasonNumber >= request.SeasonNumber.Value)
              .Any(s => s.EpisodeCount >= request.MinEpisodeCount.Value));
        }
        else
        {
          // TV shows with seasons that have at least one episode
          query = query.Where(t => t.Seasons
              .Where(s => request.ExactMatch ? s.SeasonNumber == request.SeasonNumber.Value : s.SeasonNumber >= request.SeasonNumber.Value)
              .Any(s => s.Episodes.Any()));
        }
      }
    }

    var tvShows = await query
        .OrderBy(t => t.Name)
        .ToListAsync(cancellationToken);

    var tvShowDtos = tvShows.Select(tvShow => new TvShowDto
    {
      Id = tvShow.Id,
      TmdbId = tvShow.TmdbId,
      Name = tvShow.Name,
      OriginalName = tvShow.OriginalName,
      Overview = tvShow.Overview,
      PosterPath = tvShow.PosterPath,
      BackdropPath = tvShow.BackdropPath,
      FirstAirDate = tvShow.FirstAirDate,
      NumberOfSeasons = tvShow.NumberOfSeasons,
      NumberOfEpisodes = tvShow.NumberOfEpisodes,
      Status = tvShow.Status,
      VoteAverage = tvShow.VoteAverage,
      GenresString = tvShow.GenresString
    }).ToList();

    return ApiResponse<List<TvShowDto>>.CreateSuccess(tvShowDtos);
  }
}
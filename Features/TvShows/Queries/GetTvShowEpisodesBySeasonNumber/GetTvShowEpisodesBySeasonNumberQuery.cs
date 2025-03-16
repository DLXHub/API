using API.Features.TvShows.Queries.GetSeasonEpisodes;
using API.Shared.Infrastructure;
using API.Shared.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.TvShows.Queries.GetTvShowEpisodesBySeasonNumber;

/// <summary>
/// Query to get all episodes of a specific season number for a TV show.
/// </summary>
public record GetTvShowEpisodesBySeasonNumberQuery : IRequest<ApiResponse<List<EpisodeDto>>>
{
  /// <summary>
  /// The ID of the TV show.
  /// </summary>
  public Guid TvShowId { get; init; }

  /// <summary>
  /// The season number to get episodes for.
  /// </summary>
  public int SeasonNumber { get; init; }
}

/// <summary>
/// Validator for the GetTvShowEpisodesBySeasonNumberQuery.
/// </summary>
public class GetTvShowEpisodesBySeasonNumberQueryValidator : AbstractValidator<GetTvShowEpisodesBySeasonNumberQuery>
{
  /// <summary>
  /// Initializes a new instance of the GetTvShowEpisodesBySeasonNumberQueryValidator class.
  /// </summary>
  public GetTvShowEpisodesBySeasonNumberQueryValidator()
  {
    RuleFor(x => x.TvShowId)
        .NotEmpty().WithMessage("TV show ID is required.");

    RuleFor(x => x.SeasonNumber)
        .GreaterThanOrEqualTo(0).WithMessage("Season number must be non-negative.");
  }
}

/// <summary>
/// Handler for the GetTvShowEpisodesBySeasonNumberQuery.
/// </summary>
public class GetTvShowEpisodesBySeasonNumberQueryHandler : IRequestHandler<GetTvShowEpisodesBySeasonNumberQuery, ApiResponse<List<EpisodeDto>>>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetTvShowEpisodesBySeasonNumberQueryHandler class.
  /// </summary>
  /// <param name="context">The database context.</param>
  public GetTvShowEpisodesBySeasonNumberQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the query to get all episodes of a specific season number for a TV show.
  /// </summary>
  /// <param name="request">The query.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of episodes.</returns>
  public async Task<ApiResponse<List<EpisodeDto>>> Handle(GetTvShowEpisodesBySeasonNumberQuery request, CancellationToken cancellationToken)
  {
    // Check if the TV show exists
    var tvShowExists = await _context.TvShows
        .AnyAsync(t => t.Id == request.TvShowId, cancellationToken);

    if (!tvShowExists)
    {
      return ApiResponse<List<EpisodeDto>>.CreateError("TV show not found.");
    }

    // Find the season with the specified season number for this TV show
    var season = await _context.Seasons
        .FirstOrDefaultAsync(s => s.TvShowId == request.TvShowId && s.SeasonNumber == request.SeasonNumber, cancellationToken);

    if (season == null)
    {
      return ApiResponse<List<EpisodeDto>>.CreateError($"Season {request.SeasonNumber} not found for this TV show.");
    }

    // Get all episodes for this season
    var episodes = await _context.Episodes
        .Where(e => e.SeasonId == season.Id)
        .OrderBy(e => e.EpisodeNumber)
        .ToListAsync(cancellationToken);

    var episodeDtos = episodes.Select(episode => new EpisodeDto
    {
      Id = episode.Id,
      TmdbId = episode.TmdbId,
      EpisodeNumber = episode.EpisodeNumber,
      Name = episode.Name,
      Overview = episode.Overview,
      StillPath = episode.StillPath,
      AirDate = episode.AirDate,
      Runtime = episode.Runtime,
      VoteAverage = episode.VoteAverage,
      VoteCount = episode.VoteCount
    }).ToList();

    return ApiResponse<List<EpisodeDto>>.CreateSuccess(episodeDtos);
  }
}
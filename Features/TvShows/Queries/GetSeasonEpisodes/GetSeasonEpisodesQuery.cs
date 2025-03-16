using API.Shared.Infrastructure;
using API.Shared.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.TvShows.Queries.GetSeasonEpisodes;

/// <summary>
/// DTO for an episode.
/// </summary>
public class EpisodeDto
{
  /// <summary>
  /// The ID of the episode.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// The TMDB ID of the episode.
  /// </summary>
  public int TmdbId { get; set; }

  /// <summary>
  /// The episode number within the season.
  /// </summary>
  public int EpisodeNumber { get; set; }

  /// <summary>
  /// The name of the episode.
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// The overview or description of the episode.
  /// </summary>
  public string? Overview { get; set; }

  /// <summary>
  /// The still image path for the episode.
  /// </summary>
  public string? StillPath { get; set; }

  /// <summary>
  /// The air date of the episode.
  /// </summary>
  public DateTime? AirDate { get; set; }

  /// <summary>
  /// The runtime of the episode in minutes.
  /// </summary>
  public int? Runtime { get; set; }

  /// <summary>
  /// The average vote for the episode.
  /// </summary>
  public decimal? VoteAverage { get; set; }

  /// <summary>
  /// The number of votes for the episode.
  /// </summary>
  public int? VoteCount { get; set; }
}

/// <summary>
/// Query to get all episodes of a season.
/// </summary>
public record GetSeasonEpisodesQuery : IRequest<ApiResponse<List<EpisodeDto>>>
{
  /// <summary>
  /// The ID of the season.
  /// </summary>
  public Guid SeasonId { get; init; }
}

/// <summary>
/// Validator for the GetSeasonEpisodesQuery.
/// </summary>
public class GetSeasonEpisodesQueryValidator : AbstractValidator<GetSeasonEpisodesQuery>
{
  /// <summary>
  /// Initializes a new instance of the GetSeasonEpisodesQueryValidator class.
  /// </summary>
  public GetSeasonEpisodesQueryValidator()
  {
    RuleFor(x => x.SeasonId)
        .NotEmpty().WithMessage("Season ID is required.");
  }
}

/// <summary>
/// Handler for the GetSeasonEpisodesQuery.
/// </summary>
public class GetSeasonEpisodesQueryHandler : IRequestHandler<GetSeasonEpisodesQuery, ApiResponse<List<EpisodeDto>>>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetSeasonEpisodesQueryHandler class.
  /// </summary>
  /// <param name="context">The database context.</param>
  public GetSeasonEpisodesQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the query to get all episodes of a season.
  /// </summary>
  /// <param name="request">The query.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of episodes.</returns>
  public async Task<ApiResponse<List<EpisodeDto>>> Handle(GetSeasonEpisodesQuery request, CancellationToken cancellationToken)
  {
    // Check if the season exists
    var seasonExists = await _context.Seasons
        .AnyAsync(s => s.Id == request.SeasonId, cancellationToken);

    if (!seasonExists)
    {
      return ApiResponse<List<EpisodeDto>>.CreateError("Season not found.");
    }

    var episodes = await _context.Episodes
        .Where(e => e.SeasonId == request.SeasonId)
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
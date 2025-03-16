using API.Features.TvShows.Models;
using API.Shared.Infrastructure;
using API.Shared.Models;
using API.Shared.Utilities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.TvShows.Commands.ImportTvShow;

/// <summary>
/// DTO for TV show response.
/// </summary>
public class TvShowDto
{
  /// <summary>
  /// The ID of the TV show.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// The TMDB ID of the TV show.
  /// </summary>
  public int TmdbId { get; set; }

  /// <summary>
  /// The name of the TV show.
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// The original name of the TV show.
  /// </summary>
  public string? OriginalName { get; set; }

  /// <summary>
  /// The URL-friendly slug for the TV show.
  /// </summary>
  public string? Slug { get; set; }

  /// <summary>
  /// The overview or description of the TV show.
  /// </summary>
  public string? Overview { get; set; }

  /// <summary>
  /// The poster path for the TV show.
  /// </summary>
  public string? PosterPath { get; set; }

  /// <summary>
  /// The backdrop path for the TV show.
  /// </summary>
  public string? BackdropPath { get; set; }

  /// <summary>
  /// The first air date of the TV show.
  /// </summary>
  public DateTime? FirstAirDate { get; set; }

  /// <summary>
  /// The number of seasons in the TV show.
  /// </summary>
  public int? NumberOfSeasons { get; set; }

  /// <summary>
  /// The number of episodes in the TV show.
  /// </summary>
  public int? NumberOfEpisodes { get; set; }

  /// <summary>
  /// The status of the TV show.
  /// </summary>
  public string? Status { get; set; }

  /// <summary>
  /// The average vote for the TV show.
  /// </summary>
  public decimal? VoteAverage { get; set; }

  /// <summary>
  /// The genres of the TV show.
  /// </summary>
  public string? GenresString { get; set; }
}

/// <summary>
/// Command to import a TV show from TMDB.
/// </summary>
public record ImportTvShowCommand : IRequest<ApiResponse<TvShowDto>>
{
  /// <summary>
  /// The TMDB ID of the TV show to import.
  /// </summary>
  public int TmdbId { get; init; }

  /// <summary>
  /// Whether to import seasons and episodes.
  /// </summary>
  public bool IncludeSeasons { get; init; } = false;
}

/// <summary>
/// Validator for the ImportTvShowCommand.
/// </summary>
public class ImportTvShowCommandValidator : AbstractValidator<ImportTvShowCommand>
{
  /// <summary>
  /// Initializes a new instance of the ImportTvShowCommandValidator class.
  /// </summary>
  public ImportTvShowCommandValidator()
  {
    RuleFor(x => x.TmdbId)
        .NotEmpty().WithMessage("TMDB ID is required.")
        .GreaterThan(0).WithMessage("TMDB ID must be greater than 0.");
  }
}

/// <summary>
/// Handler for the ImportTvShowCommand.
/// </summary>
public class ImportTvShowCommandHandler : IRequestHandler<ImportTvShowCommand, ApiResponse<TvShowDto>>
{
  private readonly ApplicationDbContext _context;
  private readonly IHttpContextAccessor _httpContextAccessor;

  /// <summary>
  /// Initializes a new instance of the ImportTvShowCommandHandler class.
  /// </summary>
  /// <param name="context">The database context.</param>
  /// <param name="httpContextAccessor">The HTTP context accessor.</param>
  public ImportTvShowCommandHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
  {
    _context = context;
    _httpContextAccessor = httpContextAccessor;
  }

  /// <summary>
  /// Handles the command to import a TV show from TMDB.
  /// </summary>
  /// <param name="request">The command.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The imported TV show.</returns>
  public async Task<ApiResponse<TvShowDto>> Handle(ImportTvShowCommand request, CancellationToken cancellationToken)
  {
    // Check if the TV show already exists in the database
    var existingTvShow = await _context.TvShows
        .FirstOrDefaultAsync(t => t.TmdbId == request.TmdbId, cancellationToken);

    if (existingTvShow != null)
    {
      // Return the existing TV show
      return ApiResponse<TvShowDto>.CreateSuccess(MapToDto(existingTvShow), "TV show already exists in the database.");
    }

    // In a real implementation, you would call the TMDB API here to get the TV show details
    // For this example, we'll create a dummy TV show with the provided TMDB ID

    var tvShowName = $"TV Show {request.TmdbId}";

    // Generate a slug for the TV show
    var existingSlugs = await _context.TvShows
        .Where(t => t.Slug != null)
        .Select(t => t.Slug!)
        .ToListAsync(cancellationToken);

    var slug = SlugGenerator.GenerateUniqueSlug(tvShowName, existingSlugs);

    var tvShow = new TvShow
    {
      TmdbId = request.TmdbId,
      Name = tvShowName,
      Slug = slug,
      Overview = "This is a placeholder for a TV show imported from TMDB.",
      FirstAirDate = DateTime.UtcNow.AddYears(-1),
      Status = "Returning Series",
      NumberOfSeasons = 3,
      NumberOfEpisodes = 30,
      VoteAverage = 7.5m,
      Popularity = 100.0m,
      GenresString = "Drama,Action",
      LastUpdated = DateTime.UtcNow
    };

    _context.TvShows.Add(tvShow);
    await _context.SaveChangesAsync(cancellationToken);

    // If seasons should be included, create some dummy seasons and episodes
    if (request.IncludeSeasons)
    {
      for (int i = 1; i <= tvShow.NumberOfSeasons; i++)
      {
        var season = new Season
        {
          TmdbId = request.TmdbId * 100 + i,
          TvShowId = tvShow.Id,
          SeasonNumber = i,
          Name = $"Season {i}",
          Overview = $"This is season {i} of the TV show.",
          AirDate = DateTime.UtcNow.AddYears(-1).AddMonths(i * 3),
          EpisodeCount = 10
        };

        _context.Seasons.Add(season);

        // Create episodes for this season
        for (int j = 1; j <= 10; j++)
        {
          var episode = new Episode
          {
            TmdbId = request.TmdbId * 10000 + i * 100 + j,
            SeasonId = season.Id,
            EpisodeNumber = j,
            Name = $"Episode {j}",
            Overview = $"This is episode {j} of season {i}.",
            AirDate = DateTime.UtcNow.AddYears(-1).AddMonths(i * 3).AddDays(j * 7),
            Runtime = 45,
            VoteAverage = 7.0m,
            VoteCount = 100
          };

          _context.Episodes.Add(episode);
        }
      }

      await _context.SaveChangesAsync(cancellationToken);
    }

    return ApiResponse<TvShowDto>.CreateSuccess(MapToDto(tvShow), "TV show imported successfully.");
  }

  private static TvShowDto MapToDto(TvShow tvShow)
  {
    return new TvShowDto
    {
      Id = tvShow.Id,
      TmdbId = tvShow.TmdbId,
      Name = tvShow.Name,
      OriginalName = tvShow.OriginalName,
      Slug = tvShow.Slug,
      Overview = tvShow.Overview,
      PosterPath = tvShow.PosterPath,
      BackdropPath = tvShow.BackdropPath,
      FirstAirDate = tvShow.FirstAirDate,
      NumberOfSeasons = tvShow.NumberOfSeasons,
      NumberOfEpisodes = tvShow.NumberOfEpisodes,
      Status = tvShow.Status,
      VoteAverage = tvShow.VoteAverage,
      GenresString = tvShow.GenresString
    };
  }
}
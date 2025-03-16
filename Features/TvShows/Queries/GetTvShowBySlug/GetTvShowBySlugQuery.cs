using API.Features.TvShows.Commands.ImportTvShow;
using API.Shared.Infrastructure;
using API.Shared.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.TvShows.Queries.GetTvShowBySlug;

/// <summary>
/// Query to get a TV show by slug.
/// </summary>
public record GetTvShowBySlugQuery : IRequest<ApiResponse<TvShowDto>>
{
  /// <summary>
  /// The slug of the TV show to retrieve.
  /// </summary>
  public string Slug { get; init; } = string.Empty;
}

/// <summary>
/// Validator for the GetTvShowBySlugQuery.
/// </summary>
public class GetTvShowBySlugQueryValidator : AbstractValidator<GetTvShowBySlugQuery>
{
  /// <summary>
  /// Initializes a new instance of the GetTvShowBySlugQueryValidator class.
  /// </summary>
  public GetTvShowBySlugQueryValidator()
  {
    RuleFor(x => x.Slug)
        .NotEmpty().WithMessage("TV show slug is required.");
  }
}

/// <summary>
/// Handler for the GetTvShowBySlugQuery.
/// </summary>
public class GetTvShowBySlugQueryHandler : IRequestHandler<GetTvShowBySlugQuery, ApiResponse<TvShowDto>>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetTvShowBySlugQueryHandler class.
  /// </summary>
  /// <param name="context">The database context.</param>
  public GetTvShowBySlugQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the query to get a TV show by slug.
  /// </summary>
  /// <param name="request">The query.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The TV show.</returns>
  public async Task<ApiResponse<TvShowDto>> Handle(GetTvShowBySlugQuery request, CancellationToken cancellationToken)
  {
    var tvShow = await _context.TvShows
        .AsNoTracking()
        .FirstOrDefaultAsync(t => t.Slug == request.Slug, cancellationToken);

    if (tvShow == null)
    {
      return ApiResponse<TvShowDto>.CreateError("TV show not found.");
    }

    var tvShowDto = new TvShowDto
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

    return ApiResponse<TvShowDto>.CreateSuccess(tvShowDto);
  }
}
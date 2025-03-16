using API.Features.TvShows.Commands.ImportTvShow;
using API.Shared.Infrastructure;
using API.Shared.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.TvShows.Queries.GetTvShow;

/// <summary>
/// Query to get a TV show by ID.
/// </summary>
public record GetTvShowQuery : IRequest<ApiResponse<TvShowDto>>
{
  /// <summary>
  /// The ID of the TV show to retrieve.
  /// </summary>
  public Guid Id { get; init; }
}

/// <summary>
/// Validator for the GetTvShowQuery.
/// </summary>
public class GetTvShowQueryValidator : AbstractValidator<GetTvShowQuery>
{
  /// <summary>
  /// Initializes a new instance of the GetTvShowQueryValidator class.
  /// </summary>
  public GetTvShowQueryValidator()
  {
    RuleFor(x => x.Id)
        .NotEmpty().WithMessage("TV show ID is required.");
  }
}

/// <summary>
/// Handler for the GetTvShowQuery.
/// </summary>
public class GetTvShowQueryHandler : IRequestHandler<GetTvShowQuery, ApiResponse<TvShowDto>>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetTvShowQueryHandler class.
  /// </summary>
  /// <param name="context">The database context.</param>
  public GetTvShowQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the query to get a TV show by ID.
  /// </summary>
  /// <param name="request">The query.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The TV show.</returns>
  public async Task<ApiResponse<TvShowDto>> Handle(GetTvShowQuery request, CancellationToken cancellationToken)
  {
    var tvShow = await _context.TvShows
        .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

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
using API.Features.Downloads.Models;
using API.Shared.Infrastructure;
using API.Shared.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Downloads.Commands.CreateDownload;

/// <summary>
/// Command to create a new download.
/// </summary>
public record CreateDownloadCommand : IRequest<ApiResponse<DownloadDto>>
{
  /// <summary>
  /// The title of the download.
  /// </summary>
  public string Title { get; init; } = string.Empty;

  /// <summary>
  /// The language of the download.
  /// </summary>
  public string Language { get; init; } = string.Empty;

  /// <summary>
  /// The quality of the download.
  /// </summary>
  public string Quality { get; init; } = string.Empty;

  /// <summary>
  /// The type of media this download is associated with.
  /// </summary>
  public MediaType MediaType { get; init; }

  /// <summary>
  /// The ID of the media item (movie, TV show, season, or episode).
  /// </summary>
  public Guid MediaId { get; init; }
}

/// <summary>
/// Validator for the CreateDownloadCommand.
/// </summary>
public class CreateDownloadCommandValidator : AbstractValidator<CreateDownloadCommand>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the CreateDownloadCommandValidator class.
  /// </summary>
  /// <param name="context">The database context.</param>
  public CreateDownloadCommandValidator(ApplicationDbContext context)
  {
    _context = context;

    RuleFor(x => x.Title)
        .NotEmpty().WithMessage("Title is required.")
        .MaximumLength(255).WithMessage("Title must not exceed 255 characters.");

    RuleFor(x => x.Language)
        .NotEmpty().WithMessage("Language is required.")
        .MaximumLength(50).WithMessage("Language must not exceed 50 characters.");

    RuleFor(x => x.Quality)
        .NotEmpty().WithMessage("Quality is required.")
        .MaximumLength(50).WithMessage("Quality must not exceed 50 characters.");

    RuleFor(x => x.MediaType)
        .IsInEnum().WithMessage("Invalid media type.");

    RuleFor(x => x)
        .MustAsync(ValidateMediaExists).WithMessage("The specified media item does not exist.");
  }

  private async Task<bool> ValidateMediaExists(CreateDownloadCommand command, CancellationToken cancellationToken)
  {
    return command.MediaType switch
    {
      MediaType.Movie => await _context.Movies.AnyAsync(m => m.Id == command.MediaId, cancellationToken),
      MediaType.TvShow => await _context.TvShows.AnyAsync(t => t.Id == command.MediaId, cancellationToken),
      MediaType.Season => await _context.Seasons.AnyAsync(s => s.Id == command.MediaId, cancellationToken),
      MediaType.Episode => await _context.Episodes.AnyAsync(e => e.Id == command.MediaId, cancellationToken),
      _ => false
    };
  }
}

/// <summary>
/// Handler for the CreateDownloadCommand.
/// </summary>
public class CreateDownloadCommandHandler : IRequestHandler<CreateDownloadCommand, ApiResponse<DownloadDto>>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the CreateDownloadCommandHandler class.
  /// </summary>
  /// <param name="context">The database context.</param>
  public CreateDownloadCommandHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the command to create a new download.
  /// </summary>
  /// <param name="request">The command.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The created download.</returns>
  public async Task<ApiResponse<DownloadDto>> Handle(CreateDownloadCommand request, CancellationToken cancellationToken)
  {
    var download = new Download
    {
      Title = request.Title,
      Language = request.Language,
      Quality = request.Quality,
      MediaType = request.MediaType
    };

    // Set the appropriate foreign key based on media type
    switch (request.MediaType)
    {
      case MediaType.Movie:
        download.MovieId = request.MediaId;
        break;
      case MediaType.TvShow:
        download.TvShowId = request.MediaId;
        break;
      case MediaType.Season:
        download.SeasonId = request.MediaId;
        break;
      case MediaType.Episode:
        download.EpisodeId = request.MediaId;
        break;
    }

    _context.Downloads.Add(download);
    await _context.SaveChangesAsync(cancellationToken);

    // Load the related entity for proper DTO mapping
    await LoadRelatedEntity(download, cancellationToken);

    var downloadDto = DownloadDto.FromEntity(download);
    return ApiResponse<DownloadDto>.CreateSuccess(downloadDto, "Download created successfully.");
  }

  private async Task LoadRelatedEntity(Download download, CancellationToken cancellationToken)
  {
    switch (download.MediaType)
    {
      case MediaType.Movie:
        await _context.Entry(download)
            .Reference(d => d.Movie)
            .LoadAsync(cancellationToken);
        break;
      case MediaType.TvShow:
        await _context.Entry(download)
            .Reference(d => d.TvShow)
            .LoadAsync(cancellationToken);
        break;
      case MediaType.Season:
        await _context.Entry(download)
            .Reference(d => d.Season)
            .Query()
            .Include(s => s.TvShow)
            .LoadAsync(cancellationToken);
        break;
      case MediaType.Episode:
        await _context.Entry(download)
            .Reference(d => d.Episode)
            .Query()
            .Include(e => e.Season)
            .ThenInclude(s => s.TvShow)
            .LoadAsync(cancellationToken);
        break;
    }
  }
}
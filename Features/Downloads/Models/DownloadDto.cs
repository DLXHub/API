using API.Features.Downloads.Models;

namespace API.Features.Downloads.Models;

/// <summary>
/// Data Transfer Object for a download.
/// </summary>
public class DownloadDto
{
  /// <summary>
  /// The unique identifier of the download.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// The title of the download.
  /// </summary>
  public string Title { get; set; } = string.Empty;

  /// <summary>
  /// The language of the download.
  /// </summary>
  public string Language { get; set; } = string.Empty;

  /// <summary>
  /// The quality of the download.
  /// </summary>
  public string Quality { get; set; } = string.Empty;

  /// <summary>
  /// The type of media this download is associated with.
  /// </summary>
  public MediaType MediaType { get; set; }

  /// <summary>
  /// The ID of the associated media (movie, TV show, season, or episode).
  /// </summary>
  public Guid MediaId { get; set; }

  /// <summary>
  /// The name or title of the associated media.
  /// </summary>
  public string MediaTitle { get; set; } = string.Empty;

  /// <summary>
  /// The date and time when the download was created.
  /// </summary>
  public DateTime CreatedOn { get; set; }

  /// <summary>
  /// Maps a Download entity to a DownloadDto.
  /// </summary>
  /// <param name="download">The download entity to map.</param>
  /// <returns>A new DownloadDto instance.</returns>
  public static DownloadDto FromEntity(Download download)
  {
    return new DownloadDto
    {
      Id = download.Id,
      Title = download.Title,
      Language = download.Language,
      Quality = download.Quality,
      MediaType = download.MediaType,
      MediaId = download.MediaType switch
      {
        MediaType.Movie => download.MovieId ?? Guid.Empty,
        MediaType.TvShow => download.TvShowId ?? Guid.Empty,
        MediaType.Season => download.SeasonId ?? Guid.Empty,
        MediaType.Episode => download.EpisodeId ?? Guid.Empty,
        _ => Guid.Empty
      },
      MediaTitle = download.MediaType switch
      {
        MediaType.Movie => download.Movie?.Title ?? string.Empty,
        MediaType.TvShow => download.TvShow?.Name ?? string.Empty,
        MediaType.Season => $"{download.Season?.TvShow?.Name} Season {download.Season?.SeasonNumber}",
        MediaType.Episode => $"{download.Episode?.Season?.TvShow?.Name} S{download.Episode?.Season?.SeasonNumber:D2}E{download.Episode?.EpisodeNumber:D2}",
        _ => string.Empty
      },
      CreatedOn = download.CreatedOn
    };
  }
}
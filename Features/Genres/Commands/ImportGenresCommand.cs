using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Features.Genres.Models;
using API.Features.Genres.Queries;
using API.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Genres.Commands;

/// <summary>
/// Command to import genres from TMDB
/// </summary>
public class ImportGenresCommand : IRequest<List<GenreDto>>
{
  /// <summary>
  /// The type of genres to import (movie or tv)
  /// </summary>
  public string Type { get; set; } = "movie";
}

/// <summary>
/// Handler for the ImportGenresCommand
/// </summary>
public class ImportGenresCommandHandler : IRequestHandler<ImportGenresCommand, List<GenreDto>>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the ImportGenresCommandHandler class
  /// </summary>
  /// <param name="context">The database context</param>
  public ImportGenresCommandHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the ImportGenresCommand
  /// </summary>
  /// <param name="request">The command</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>A list of imported genres</returns>
  public async Task<List<GenreDto>> Handle(ImportGenresCommand request, CancellationToken cancellationToken)
  {
    // In a real application, you would call the TMDB API here
    // For now, we'll create some dummy genres
    var genresToImport = GetDummyGenres(request.Type);

    var existingGenres = await _context.Genres
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    var newGenres = new List<Genre>();

    foreach (var genreData in genresToImport)
    {
      var existingGenre = existingGenres.FirstOrDefault(g => g.TmdbId == genreData.TmdbId);

      if (existingGenre == null)
      {
        var newGenre = new Genre
        {
          TmdbId = genreData.TmdbId,
          Name = genreData.Name
        };

        newGenres.Add(newGenre);
        _context.Genres.Add(newGenre);
      }
      else if (existingGenre.Name != genreData.Name)
      {
        existingGenre.Name = genreData.Name;
        _context.Genres.Update(existingGenre);
      }
    }

    await _context.SaveChangesAsync(cancellationToken);

    // Return all genres, including the newly imported ones
    var allGenres = await _context.Genres
        .AsNoTracking()
        .OrderBy(g => g.Name)
        .ToListAsync(cancellationToken);

    return allGenres.Select(GenreDto.FromEntity).ToList();
  }

  private List<(int TmdbId, string Name)> GetDummyGenres(string type)
  {
    if (type.ToLower() == "movie")
    {
      return new List<(int TmdbId, string Name)>
            {
                (28, "Action"),
                (12, "Adventure"),
                (16, "Animation"),
                (35, "Comedy"),
                (80, "Crime"),
                (99, "Documentary"),
                (18, "Drama"),
                (10751, "Family"),
                (14, "Fantasy"),
                (36, "History"),
                (27, "Horror"),
                (10402, "Music"),
                (9648, "Mystery"),
                (10749, "Romance"),
                (878, "Science Fiction"),
                (10770, "TV Movie"),
                (53, "Thriller"),
                (10752, "War"),
                (37, "Western")
            };
    }
    else // TV genres
    {
      return new List<(int TmdbId, string Name)>
            {
                (10759, "Action & Adventure"),
                (16, "Animation"),
                (35, "Comedy"),
                (80, "Crime"),
                (99, "Documentary"),
                (18, "Drama"),
                (10751, "Family"),
                (10762, "Kids"),
                (9648, "Mystery"),
                (10763, "News"),
                (10764, "Reality"),
                (10765, "Sci-Fi & Fantasy"),
                (10766, "Soap"),
                (10767, "Talk"),
                (10768, "War & Politics"),
                (37, "Western")
            };
    }
  }
}
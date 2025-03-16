using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using API.Features.Movies.Models;
using API.Features.People.Models;
using API.Features.People.Queries;
using API.Features.TvShows.Models;
using API.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.People.Commands;

/// <summary>
/// Command to import a person's credits from TMDB
/// </summary>
public class ImportPersonCreditsCommand : IRequest<PersonCombinedCreditsDto?>
{
  /// <summary>
  /// The ID of the person whose credits to import
  /// </summary>
  public Guid PersonId { get; set; }
}

/// <summary>
/// Handler for the ImportPersonCreditsCommand
/// </summary>
public class ImportPersonCreditsCommandHandler : IRequestHandler<ImportPersonCreditsCommand, PersonCombinedCreditsDto?>
{
  private readonly ApplicationDbContext _context;
  // In a real application, you would inject a TMDB API client here

  /// <summary>
  /// Initializes a new instance of the ImportPersonCreditsCommandHandler class
  /// </summary>
  /// <param name="context">The database context</param>
  public ImportPersonCreditsCommandHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the ImportPersonCreditsCommand
  /// </summary>
  /// <param name="request">The command</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The imported person credits DTO or null if the person is not found</returns>
  public async Task<PersonCombinedCreditsDto?> Handle(ImportPersonCreditsCommand request, CancellationToken cancellationToken)
  {
    var person = await _context.People
        .FirstOrDefaultAsync(p => p.Id == request.PersonId, cancellationToken);

    if (person == null)
    {
      return null;
    }

    // In a real application, you would fetch the credits data from the TMDB API
    // For this example, we'll create dummy credits

    // Create some dummy movies if they don't exist
    var movies = await EnsureDummyMoviesExist(cancellationToken);

    // Create some dummy TV shows if they don't exist
    var tvShows = await EnsureDummyTvShowsExist(cancellationToken);

    // Create movie cast credits
    var movieCast = await CreateDummyMovieCastCredits(person, movies, cancellationToken);

    // Create movie crew credits
    var movieCrew = await CreateDummyMovieCrewCredits(person, movies, cancellationToken);

    // Create TV show cast credits
    var tvCast = await CreateDummyTvShowCastCredits(person, tvShows, cancellationToken);

    // Create TV show crew credits
    var tvCrew = await CreateDummyTvShowCrewCredits(person, tvShows, cancellationToken);

    return PersonCombinedCreditsDto.FromEntity(person, movieCast, movieCrew, tvCast, tvCrew);
  }

  private async Task<List<Movie>> EnsureDummyMoviesExist(CancellationToken cancellationToken)
  {
    var movies = new List<Movie>();

    for (int i = 1; i <= 3; i++)
    {
      var tmdbId = 1000 + i;
      var movie = await _context.Movies.FirstOrDefaultAsync(m => m.TmdbId == tmdbId, cancellationToken);

      if (movie == null)
      {
        movie = new Movie
        {
          TmdbId = tmdbId,
          Title = $"Movie {i}",
          Overview = $"This is a dummy movie {i} for testing credits.",
          PosterPath = $"/path/to/poster/{tmdbId}.jpg",
          ReleaseDate = DateTime.UtcNow.AddYears(-i),
          Runtime = 120,
          VoteAverage = 7.5m,
          VoteCount = 1000
        };

        _context.Movies.Add(movie);
        await _context.SaveChangesAsync(cancellationToken);
      }

      movies.Add(movie);
    }

    return movies;
  }

  private async Task<List<TvShow>> EnsureDummyTvShowsExist(CancellationToken cancellationToken)
  {
    var tvShows = new List<TvShow>();

    for (int i = 1; i <= 3; i++)
    {
      var tmdbId = 2000 + i;
      var tvShow = await _context.TvShows.FirstOrDefaultAsync(t => t.TmdbId == tmdbId, cancellationToken);

      if (tvShow == null)
      {
        tvShow = new TvShow
        {
          TmdbId = tmdbId,
          Name = $"TV Show {i}",
          Overview = $"This is a dummy TV show {i} for testing credits.",
          PosterPath = $"/path/to/poster/{tmdbId}.jpg",
          FirstAirDate = DateTime.UtcNow.AddYears(-i),
          LastAirDate = DateTime.UtcNow.AddMonths(-i),
          Status = "Ended",
          NumberOfSeasons = 3,
          NumberOfEpisodes = 30,
          VoteAverage = 8.0m,
          VoteCount = 1500
        };

        _context.TvShows.Add(tvShow);
        await _context.SaveChangesAsync(cancellationToken);
      }

      tvShows.Add(tvShow);
    }

    return tvShows;
  }

  private async Task<List<MovieCast>> CreateDummyMovieCastCredits(Person person, List<Movie> movies, CancellationToken cancellationToken)
  {
    var credits = new List<MovieCast>();

    // Delete existing credits to avoid duplicates
    var existingCredits = await _context.MovieCast
        .Where(mc => mc.PersonId == person.Id)
        .ToListAsync(cancellationToken);

    if (existingCredits.Any())
    {
      _context.MovieCast.RemoveRange(existingCredits);
      await _context.SaveChangesAsync(cancellationToken);
    }

    // Create new credits
    for (int i = 0; i < movies.Count; i++)
    {
      var credit = new MovieCast
      {
        PersonId = person.Id,
        MovieId = movies[i].Id,
        Character = $"Character {i + 1}",
        Order = i,
        CreditId = $"mc_{person.Id}_{movies[i].Id}"
      };

      _context.MovieCast.Add(credit);
      credits.Add(credit);
    }

    await _context.SaveChangesAsync(cancellationToken);
    return credits;
  }

  private async Task<List<MovieCrew>> CreateDummyMovieCrewCredits(Person person, List<Movie> movies, CancellationToken cancellationToken)
  {
    var credits = new List<MovieCrew>();

    // Delete existing credits to avoid duplicates
    var existingCredits = await _context.MovieCrew
        .Where(mc => mc.PersonId == person.Id)
        .ToListAsync(cancellationToken);

    if (existingCredits.Any())
    {
      _context.MovieCrew.RemoveRange(existingCredits);
      await _context.SaveChangesAsync(cancellationToken);
    }

    // Create new credits
    var departments = new[] { "Directing", "Writing", "Production" };
    var jobs = new[] { "Director", "Screenplay", "Producer" };

    for (int i = 0; i < Math.Min(movies.Count, departments.Length); i++)
    {
      var credit = new MovieCrew
      {
        PersonId = person.Id,
        MovieId = movies[i].Id,
        Department = departments[i],
        Job = jobs[i],
        CreditId = $"mcr_{person.Id}_{movies[i].Id}_{jobs[i]}"
      };

      _context.MovieCrew.Add(credit);
      credits.Add(credit);
    }

    await _context.SaveChangesAsync(cancellationToken);
    return credits;
  }

  private async Task<List<TvShowCast>> CreateDummyTvShowCastCredits(Person person, List<TvShow> tvShows, CancellationToken cancellationToken)
  {
    var credits = new List<TvShowCast>();

    // Delete existing credits to avoid duplicates
    var existingCredits = await _context.TvShowCast
        .Where(tc => tc.PersonId == person.Id)
        .ToListAsync(cancellationToken);

    if (existingCredits.Any())
    {
      _context.TvShowCast.RemoveRange(existingCredits);
      await _context.SaveChangesAsync(cancellationToken);
    }

    // Create new credits
    for (int i = 0; i < tvShows.Count; i++)
    {
      var credit = new TvShowCast
      {
        PersonId = person.Id,
        TvShowId = tvShows[i].Id,
        Character = $"TV Character {i + 1}",
        Order = i,
        CreditId = $"tc_{person.Id}_{tvShows[i].Id}"
      };

      _context.TvShowCast.Add(credit);
      credits.Add(credit);
    }

    await _context.SaveChangesAsync(cancellationToken);
    return credits;
  }

  private async Task<List<TvShowCrew>> CreateDummyTvShowCrewCredits(Person person, List<TvShow> tvShows, CancellationToken cancellationToken)
  {
    var credits = new List<TvShowCrew>();

    // Delete existing credits to avoid duplicates
    var existingCredits = await _context.TvShowCrew
        .Where(tc => tc.PersonId == person.Id)
        .ToListAsync(cancellationToken);

    if (existingCredits.Any())
    {
      _context.TvShowCrew.RemoveRange(existingCredits);
      await _context.SaveChangesAsync(cancellationToken);
    }

    // Create new credits
    var departments = new[] { "Directing", "Writing", "Production" };
    var jobs = new[] { "Director", "Writer", "Executive Producer" };

    for (int i = 0; i < Math.Min(tvShows.Count, departments.Length); i++)
    {
      var credit = new TvShowCrew
      {
        PersonId = person.Id,
        TvShowId = tvShows[i].Id,
        Department = departments[i],
        Job = jobs[i],
        CreditId = $"tcr_{person.Id}_{tvShows[i].Id}_{jobs[i]}"
      };

      _context.TvShowCrew.Add(credit);
      credits.Add(credit);
    }

    await _context.SaveChangesAsync(cancellationToken);
    return credits;
  }
}
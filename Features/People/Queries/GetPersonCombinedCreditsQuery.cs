using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.People.Queries;

/// <summary>
/// Query to get a person's combined credits (movie and TV, cast and crew)
/// </summary>
public class GetPersonCombinedCreditsQuery : IRequest<PersonCombinedCreditsDto?>
{
  /// <summary>
  /// The ID of the person
  /// </summary>
  public Guid PersonId { get; set; }
}

/// <summary>
/// Handler for the GetPersonCombinedCreditsQuery
/// </summary>
public class GetPersonCombinedCreditsQueryHandler : IRequestHandler<GetPersonCombinedCreditsQuery, PersonCombinedCreditsDto?>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetPersonCombinedCreditsQueryHandler class
  /// </summary>
  /// <param name="context">The database context</param>
  public GetPersonCombinedCreditsQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the GetPersonCombinedCreditsQuery
  /// </summary>
  /// <param name="request">The query</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The person's combined credits or null if the person is not found</returns>
  public async Task<PersonCombinedCreditsDto?> Handle(GetPersonCombinedCreditsQuery request, CancellationToken cancellationToken)
  {
    var person = await _context.People
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.Id == request.PersonId, cancellationToken);

    if (person == null)
    {
      return null;
    }

    var movieCast = await _context.MovieCast
        .AsNoTracking()
        .Include(mc => mc.Movie)
        .Where(mc => mc.PersonId == request.PersonId)
        .OrderByDescending(mc => mc.Movie != null ? mc.Movie.ReleaseDate : null)
        .ThenBy(mc => mc.Order)
        .ToListAsync(cancellationToken);

    var movieCrew = await _context.MovieCrew
        .AsNoTracking()
        .Include(mc => mc.Movie)
        .Where(mc => mc.PersonId == request.PersonId)
        .OrderByDescending(mc => mc.Movie != null ? mc.Movie.ReleaseDate : null)
        .ThenBy(mc => mc.Department)
        .ThenBy(mc => mc.Job)
        .ToListAsync(cancellationToken);

    var tvCast = await _context.TvShowCast
        .AsNoTracking()
        .Include(tc => tc.TvShow)
        .Where(tc => tc.PersonId == request.PersonId)
        .OrderByDescending(tc => tc.TvShow != null ? tc.TvShow.FirstAirDate : null)
        .ThenBy(tc => tc.Order)
        .ToListAsync(cancellationToken);

    var tvCrew = await _context.TvShowCrew
        .AsNoTracking()
        .Include(tc => tc.TvShow)
        .Where(tc => tc.PersonId == request.PersonId)
        .OrderByDescending(tc => tc.TvShow != null ? tc.TvShow.FirstAirDate : null)
        .ThenBy(tc => tc.Department)
        .ThenBy(tc => tc.Job)
        .ToListAsync(cancellationToken);

    return PersonCombinedCreditsDto.FromEntity(person, movieCast, movieCrew, tvCast, tvCrew);
  }
}
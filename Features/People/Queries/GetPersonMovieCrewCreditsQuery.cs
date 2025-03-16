using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.People.Queries;

/// <summary>
/// Query to get a person's movie crew credits
/// </summary>
public class GetPersonMovieCrewCreditsQuery : IRequest<List<MovieCrewCreditDto>>
{
  /// <summary>
  /// The ID of the person
  /// </summary>
  public Guid PersonId { get; set; }
}

/// <summary>
/// Handler for the GetPersonMovieCrewCreditsQuery
/// </summary>
public class GetPersonMovieCrewCreditsQueryHandler : IRequestHandler<GetPersonMovieCrewCreditsQuery, List<MovieCrewCreditDto>>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetPersonMovieCrewCreditsQueryHandler class
  /// </summary>
  /// <param name="context">The database context</param>
  public GetPersonMovieCrewCreditsQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the GetPersonMovieCrewCreditsQuery
  /// </summary>
  /// <param name="request">The query</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>A list of movie crew credits</returns>
  public async Task<List<MovieCrewCreditDto>> Handle(GetPersonMovieCrewCreditsQuery request, CancellationToken cancellationToken)
  {
    var credits = await _context.MovieCrew
        .AsNoTracking()
        .Include(mc => mc.Movie)
        .Where(mc => mc.PersonId == request.PersonId)
        .OrderByDescending(mc => mc.Movie != null ? mc.Movie.ReleaseDate : null)
        .ThenBy(mc => mc.Department)
        .ThenBy(mc => mc.Job)
        .ToListAsync(cancellationToken);

    return credits.Select(MovieCrewCreditDto.FromEntity).ToList();
  }
}
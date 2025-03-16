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
/// Query to get a person's TV show crew credits
/// </summary>
public class GetPersonTvShowCrewCreditsQuery : IRequest<List<TvShowCrewCreditDto>>
{
  /// <summary>
  /// The ID of the person
  /// </summary>
  public Guid PersonId { get; set; }
}

/// <summary>
/// Handler for the GetPersonTvShowCrewCreditsQuery
/// </summary>
public class GetPersonTvShowCrewCreditsQueryHandler : IRequestHandler<GetPersonTvShowCrewCreditsQuery, List<TvShowCrewCreditDto>>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetPersonTvShowCrewCreditsQueryHandler class
  /// </summary>
  /// <param name="context">The database context</param>
  public GetPersonTvShowCrewCreditsQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the GetPersonTvShowCrewCreditsQuery
  /// </summary>
  /// <param name="request">The query</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>A list of TV show crew credits</returns>
  public async Task<List<TvShowCrewCreditDto>> Handle(GetPersonTvShowCrewCreditsQuery request, CancellationToken cancellationToken)
  {
    var credits = await _context.TvShowCrew
        .AsNoTracking()
        .Include(tc => tc.TvShow)
        .Where(tc => tc.PersonId == request.PersonId)
        .OrderByDescending(tc => tc.TvShow != null ? tc.TvShow.FirstAirDate : null)
        .ThenBy(tc => tc.Department)
        .ThenBy(tc => tc.Job)
        .ToListAsync(cancellationToken);

    return credits.Select(TvShowCrewCreditDto.FromEntity).ToList();
  }
}
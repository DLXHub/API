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
/// Query to get a person's TV show cast credits
/// </summary>
public class GetPersonTvShowCastCreditsQuery : IRequest<List<TvShowCastCreditDto>>
{
  /// <summary>
  /// The ID of the person
  /// </summary>
  public Guid PersonId { get; set; }
}

/// <summary>
/// Handler for the GetPersonTvShowCastCreditsQuery
/// </summary>
public class GetPersonTvShowCastCreditsQueryHandler : IRequestHandler<GetPersonTvShowCastCreditsQuery, List<TvShowCastCreditDto>>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetPersonTvShowCastCreditsQueryHandler class
  /// </summary>
  /// <param name="context">The database context</param>
  public GetPersonTvShowCastCreditsQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the GetPersonTvShowCastCreditsQuery
  /// </summary>
  /// <param name="request">The query</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>A list of TV show cast credits</returns>
  public async Task<List<TvShowCastCreditDto>> Handle(GetPersonTvShowCastCreditsQuery request, CancellationToken cancellationToken)
  {
    var credits = await _context.TvShowCast
        .AsNoTracking()
        .Include(tc => tc.TvShow)
        .Where(tc => tc.PersonId == request.PersonId)
        .OrderByDescending(tc => tc.TvShow != null ? tc.TvShow.FirstAirDate : null)
        .ThenBy(tc => tc.Order)
        .ToListAsync(cancellationToken);

    return credits.Select(TvShowCastCreditDto.FromEntity).ToList();
  }
}
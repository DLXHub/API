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
/// Query to get a person's movie cast credits
/// </summary>
public class GetPersonMovieCastCreditsQuery : IRequest<List<MovieCastCreditDto>>
{
  /// <summary>
  /// The ID of the person
  /// </summary>
  public Guid PersonId { get; set; }
}

/// <summary>
/// Handler for the GetPersonMovieCastCreditsQuery
/// </summary>
public class GetPersonMovieCastCreditsQueryHandler : IRequestHandler<GetPersonMovieCastCreditsQuery, List<MovieCastCreditDto>>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetPersonMovieCastCreditsQueryHandler class
  /// </summary>
  /// <param name="context">The database context</param>
  public GetPersonMovieCastCreditsQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the GetPersonMovieCastCreditsQuery
  /// </summary>
  /// <param name="request">The query</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>A list of movie cast credits</returns>
  public async Task<List<MovieCastCreditDto>> Handle(GetPersonMovieCastCreditsQuery request, CancellationToken cancellationToken)
  {
    var credits = await _context.MovieCast
        .AsNoTracking()
        .Include(mc => mc.Movie)
        .Where(mc => mc.PersonId == request.PersonId)
        .OrderByDescending(mc => mc.Movie != null ? mc.Movie.ReleaseDate : null)
        .ThenBy(mc => mc.Order)
        .ToListAsync(cancellationToken);

    return credits.Select(MovieCastCreditDto.FromEntity).ToList();
  }
}
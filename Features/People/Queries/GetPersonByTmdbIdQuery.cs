using System.Threading;
using System.Threading.Tasks;
using API.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.People.Queries;

/// <summary>
/// Query to get a person by TMDB ID
/// </summary>
public class GetPersonByTmdbIdQuery : IRequest<PersonDto?>
{
  /// <summary>
  /// The TMDB ID of the person to retrieve
  /// </summary>
  public int TmdbId { get; set; }
}

/// <summary>
/// Handler for the GetPersonByTmdbIdQuery
/// </summary>
public class GetPersonByTmdbIdQueryHandler : IRequestHandler<GetPersonByTmdbIdQuery, PersonDto?>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetPersonByTmdbIdQueryHandler class
  /// </summary>
  /// <param name="context">The database context</param>
  public GetPersonByTmdbIdQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the GetPersonByTmdbIdQuery
  /// </summary>
  /// <param name="request">The query</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The person DTO or null if not found</returns>
  public async Task<PersonDto?> Handle(GetPersonByTmdbIdQuery request, CancellationToken cancellationToken)
  {
    var person = await _context.People
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.TmdbId == request.TmdbId, cancellationToken);

    if (person == null)
    {
      return null;
    }

    return PersonDto.FromEntity(person);
  }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using API.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.People.Queries;

/// <summary>
/// Query to get a person by ID
/// </summary>
public class GetPersonQuery : IRequest<PersonDto?>
{
  /// <summary>
  /// The ID of the person to retrieve
  /// </summary>
  public Guid Id { get; set; }
}

/// <summary>
/// Handler for the GetPersonQuery
/// </summary>
public class GetPersonQueryHandler : IRequestHandler<GetPersonQuery, PersonDto?>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the GetPersonQueryHandler class
  /// </summary>
  /// <param name="context">The database context</param>
  public GetPersonQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the GetPersonQuery
  /// </summary>
  /// <param name="request">The query</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The person DTO or null if not found</returns>
  public async Task<PersonDto?> Handle(GetPersonQuery request, CancellationToken cancellationToken)
  {
    var person = await _context.People
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

    if (person == null)
    {
      return null;
    }

    return PersonDto.FromEntity(person);
  }
}
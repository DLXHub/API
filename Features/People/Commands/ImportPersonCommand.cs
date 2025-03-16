using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using API.Features.People.Models;
using API.Features.People.Queries;
using API.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.People.Commands;

/// <summary>
/// Command to import a person from TMDB
/// </summary>
public class ImportPersonCommand : IRequest<PersonDto>
{
  /// <summary>
  /// The TMDB ID of the person to import
  /// </summary>
  public int TmdbId { get; set; }
}

/// <summary>
/// Handler for the ImportPersonCommand
/// </summary>
public class ImportPersonCommandHandler : IRequestHandler<ImportPersonCommand, PersonDto>
{
  private readonly ApplicationDbContext _context;
  // In a real application, you would inject a TMDB API client here

  /// <summary>
  /// Initializes a new instance of the ImportPersonCommandHandler class
  /// </summary>
  /// <param name="context">The database context</param>
  public ImportPersonCommandHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the ImportPersonCommand
  /// </summary>
  /// <param name="request">The command</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The imported person DTO</returns>
  public async Task<PersonDto> Handle(ImportPersonCommand request, CancellationToken cancellationToken)
  {
    // Check if the person already exists
    var existingPerson = await _context.People
        .FirstOrDefaultAsync(p => p.TmdbId == request.TmdbId, cancellationToken);

    if (existingPerson != null)
    {
      // Update the existing person with the latest data from TMDB
      // In a real application, you would fetch the data from the TMDB API
      existingPerson.LastUpdated = DateTime.UtcNow;
      await _context.SaveChangesAsync(cancellationToken);
      return PersonDto.FromEntity(existingPerson);
    }

    // In a real application, you would fetch the person data from the TMDB API
    // For this example, we'll create a dummy person
    var person = new Person
    {
      TmdbId = request.TmdbId,
      Name = $"Person {request.TmdbId}",
      ProfilePath = $"/path/to/profile/{request.TmdbId}.jpg",
      Gender = 0,
      Biography = "This is a placeholder biography for a person imported from TMDB.",
      Birthday = DateTime.UtcNow.AddYears(-30),
      PlaceOfBirth = "Hollywood, USA",
      Popularity = 5.0m,
      KnownForDepartment = "Acting",
      AlsoKnownAs = new List<string> { "Alias 1", "Alias 2" },
      LastUpdated = DateTime.UtcNow
    };

    _context.People.Add(person);
    await _context.SaveChangesAsync(cancellationToken);

    return PersonDto.FromEntity(person);
  }
}
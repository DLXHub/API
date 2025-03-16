using API.Features.Collections.Models;
using API.Shared.Infrastructure;
using API.Shared.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Collections.Commands.CreateCollection;

/// <summary>
/// Command to create a new collection.
/// </summary>
public record CreateCollectionCommand : IRequest<ApiResponse<CollectionDto>>
{
  /// <summary>
  /// The name of the collection.
  /// </summary>
  public string Name { get; init; } = string.Empty;

  /// <summary>
  /// The description of the collection.
  /// </summary>
  public string? Description { get; init; }

  /// <summary>
  /// Indicates whether the collection is public or private.
  /// </summary>
  public bool IsPublic { get; init; }
}

/// <summary>
/// DTO for collection response.
/// </summary>
public class CollectionDto
{
  /// <summary>
  /// The ID of the collection.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// The name of the collection.
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// The description of the collection.
  /// </summary>
  public string? Description { get; set; }

  /// <summary>
  /// Indicates whether the collection is public or private.
  /// </summary>
  public bool IsPublic { get; set; }

  /// <summary>
  /// The ID of the owner.
  /// </summary>
  public string OwnerId { get; set; } = string.Empty;

  /// <summary>
  /// The date when the collection was created.
  /// </summary>
  public DateTime CreatedOn { get; set; }

  /// <summary>
  /// The number of movies in the collection.
  /// </summary>
  public int MovieCount { get; set; }
}

/// <summary>
/// Validator for the CreateCollectionCommand.
/// </summary>
public class CreateCollectionCommandValidator : AbstractValidator<CreateCollectionCommand>
{
  /// <summary>
  /// Initializes a new instance of the CreateCollectionCommandValidator class.
  /// </summary>
  public CreateCollectionCommandValidator()
  {
    RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Name is required.")
        .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

    RuleFor(x => x.Description)
        .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
  }
}

/// <summary>
/// Handler for the CreateCollectionCommand.
/// </summary>
public class CreateCollectionCommandHandler : IRequestHandler<CreateCollectionCommand, ApiResponse<CollectionDto>>
{
  private readonly ApplicationDbContext _context;
  private readonly IHttpContextAccessor _httpContextAccessor;

  /// <summary>
  /// Initializes a new instance of the CreateCollectionCommandHandler class.
  /// </summary>
  /// <param name="context">The database context.</param>
  /// <param name="httpContextAccessor">The HTTP context accessor.</param>
  public CreateCollectionCommandHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
  {
    _context = context;
    _httpContextAccessor = httpContextAccessor;
  }

  /// <summary>
  /// Handles the command to create a new collection.
  /// </summary>
  /// <param name="request">The command.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The created collection.</returns>
  public async Task<ApiResponse<CollectionDto>> Handle(CreateCollectionCommand request, CancellationToken cancellationToken)
  {
    var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
    {
      return ApiResponse<CollectionDto>.CreateError("User not authenticated.");
    }

    var collection = new Collection
    {
      Name = request.Name,
      Description = request.Description,
      IsPublic = request.IsPublic,
      OwnerId = userId
    };

    _context.Collections.Add(collection);
    await _context.SaveChangesAsync(cancellationToken);

    var collectionDto = new CollectionDto
    {
      Id = collection.Id,
      Name = collection.Name,
      Description = collection.Description,
      IsPublic = collection.IsPublic,
      OwnerId = collection.OwnerId,
      CreatedOn = collection.CreatedOn,
      MovieCount = 0
    };

    return ApiResponse<CollectionDto>.CreateSuccess(collectionDto, "Collection created successfully.");
  }
}
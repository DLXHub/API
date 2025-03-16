using System.Linq;
using System.Text.Json;
using API.Features.Pages.Models;
using API.Shared.Infrastructure;
using API.Shared.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Pages.Commands.CreatePage;

/// <summary>
/// Command to create a new page.
/// </summary>
public record CreatePageCommand : IRequest<ApiResponse<PageDto>>
{
  /// <summary>
  /// The title of the page.
  /// </summary>
  public string Title { get; init; } = string.Empty;

  /// <summary>
  /// The SEO-friendly title of the page.
  /// </summary>
  public string? SeoTitle { get; init; }

  /// <summary>
  /// The SEO meta description of the page.
  /// </summary>
  public string? MetaDescription { get; init; }

  /// <summary>
  /// The URL slug for the page.
  /// </summary>
  public string Slug { get; init; } = string.Empty;

  /// <summary>
  /// The internal link target key used for routing and navigation.
  /// </summary>
  public string LinkTarget { get; init; } = string.Empty;

  /// <summary>
  /// The initial components for the page.
  /// </summary>
  public List<PageComponentDto>? Components { get; init; }
}

/// <summary>
/// Validator for the CreatePageCommand.
/// </summary>
public class CreatePageCommandValidator : AbstractValidator<CreatePageCommand>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the CreatePageCommandValidator class.
  /// </summary>
  public CreatePageCommandValidator(ApplicationDbContext context)
  {
    _context = context;

    RuleFor(x => x.Title)
        .NotEmpty()
        .MaximumLength(255);

    RuleFor(x => x.SeoTitle)
        .MaximumLength(255)
        .When(x => x.SeoTitle != null);

    RuleFor(x => x.MetaDescription)
        .MaximumLength(500)
        .When(x => x.MetaDescription != null);

    RuleFor(x => x.Slug)
        .NotEmpty()
        .MaximumLength(255)
        .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
        .WithMessage("Slug must be lowercase, contain only letters, numbers, and hyphens, and cannot start or end with a hyphen.")
        .MustAsync(BeUniqueSlug)
        .WithMessage("A page with this slug already exists.");

    RuleFor(x => x.LinkTarget)
        .NotEmpty()
        .MaximumLength(100)
        .Matches("^[a-zA-Z0-9_]+$")
        .WithMessage("Link target must contain only letters, numbers, and underscores.")
        .MustAsync(BeUniqueLinkTarget)
        .WithMessage("A page with this link target already exists.");

    RuleFor(x => x.Components)
        .Must(BeValidComponents)
        .When(x => x.Components != null && x.Components.Any())
        .WithMessage("Invalid component configuration.");
  }

  private async Task<bool> BeUniqueSlug(string slug, CancellationToken cancellationToken)
  {
    return !await _context.Pages.AnyAsync(p => p.Slug == slug, cancellationToken);
  }

  private async Task<bool> BeUniqueLinkTarget(string linkTarget, CancellationToken cancellationToken)
  {
    return !await _context.Pages.AnyAsync(p => p.LinkTarget == linkTarget, cancellationToken);
  }

  private bool BeValidComponents(List<PageComponentDto>? components)
  {
    if (components == null) return true;

    try
    {
      foreach (var component in components)
      {
        if (string.IsNullOrWhiteSpace(component.Type)) return false;
        if (string.IsNullOrWhiteSpace(component.ComponentId)) return false;
        if (component.Configuration == null) return false;
      }
      return true;
    }
    catch
    {
      return false;
    }
  }
}

/// <summary>
/// Handler for the CreatePageCommand.
/// </summary>
public class CreatePageCommandHandler : IRequestHandler<CreatePageCommand, ApiResponse<PageDto>>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the CreatePageCommandHandler class.
  /// </summary>
  public CreatePageCommandHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the CreatePageCommand.
  /// </summary>
  public async Task<ApiResponse<PageDto>> Handle(CreatePageCommand request, CancellationToken cancellationToken)
  {
    var page = new Page
    {
      Title = request.Title,
      SeoTitle = request.SeoTitle,
      MetaDescription = request.MetaDescription,
      Slug = request.Slug,
      LinkTarget = request.LinkTarget,
      Status = PageStatus.Draft,
      IsPublished = false
    };

    if (request.Components != null && request.Components.Any())
    {
      var components = request.Components.Select(c => new PageComponent
      {
        Type = c.Type,
        Configuration = c.Configuration,
        Order = c.Order,
        ComponentId = c.ComponentId
      }).ToList();

      page.SetComponents(components);
    }

    _context.Pages.Add(page);
    await _context.SaveChangesAsync(cancellationToken);

    return ApiResponse<PageDto>.CreateSuccess(PageDto.FromEntity(page));
  }
}
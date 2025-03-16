using System.Text.Json;
using API.Features.Pages.Models;
using API.Shared.Infrastructure;
using API.Shared.Models;
using Ardalis.GuardClauses;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Pages.Commands.UpdatePage;

/// <summary>
/// Command to update an existing page.
/// </summary>
public record UpdatePageCommand : IRequest<ApiResponse<PageDto>>
{
  /// <summary>
  /// The ID of the page to update.
  /// </summary>
  public Guid Id { get; init; }

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
  /// The components for the page.
  /// </summary>
  public List<PageComponentDto> Components { get; init; } = new();
}

/// <summary>
/// Validator for the UpdatePageCommand.
/// </summary>
public class UpdatePageCommandValidator : AbstractValidator<UpdatePageCommand>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the UpdatePageCommandValidator class.
  /// </summary>
  public UpdatePageCommandValidator(ApplicationDbContext context)
  {
    _context = context;

    RuleFor(x => x.Id)
        .NotEmpty()
        .MustAsync(BeExistingPage)
        .WithMessage("Page not found.");

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
        .MustAsync(BeUniqueSlugForUpdate)
        .WithMessage("A page with this slug already exists.");

    RuleFor(x => x.LinkTarget)
        .NotEmpty()
        .MaximumLength(100)
        .Matches("^[a-zA-Z0-9_]+$")
        .WithMessage("Link target must contain only letters, numbers, and underscores.")
        .MustAsync(BeUniqueLinkTargetForUpdate)
        .WithMessage("A page with this link target already exists.");

    RuleForEach(x => x.Components)
        .Must(BeValidComponent)
        .WithMessage("Invalid component configuration.");
  }

  private async Task<bool> BeExistingPage(Guid id, CancellationToken cancellationToken)
  {
    return await _context.Pages.AnyAsync(p => p.Id == id, cancellationToken);
  }

  private async Task<bool> BeUniqueSlugForUpdate(UpdatePageCommand command, string slug, CancellationToken cancellationToken)
  {
    return !await _context.Pages.AnyAsync(p => p.Slug == slug && p.Id != command.Id, cancellationToken);
  }

  private async Task<bool> BeUniqueLinkTargetForUpdate(UpdatePageCommand command, string linkTarget, CancellationToken cancellationToken)
  {
    return !await _context.Pages.AnyAsync(p => p.LinkTarget == linkTarget && p.Id != command.Id, cancellationToken);
  }

  private bool BeValidComponent(PageComponentDto component)
  {
    return !string.IsNullOrWhiteSpace(component.Type) &&
           !string.IsNullOrWhiteSpace(component.ComponentId) &&
           component.Configuration != null;
  }
}

/// <summary>
/// Handler for the UpdatePageCommand.
/// </summary>
public class UpdatePageCommandHandler : IRequestHandler<UpdatePageCommand, ApiResponse<PageDto>>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the UpdatePageCommandHandler class.
  /// </summary>
  public UpdatePageCommandHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the UpdatePageCommand.
  /// </summary>
  public async Task<ApiResponse<PageDto>> Handle(UpdatePageCommand request, CancellationToken cancellationToken)
  {
    var page = await _context.Pages.FindAsync(new object[] { request.Id }, cancellationToken);
    Guard.Against.NotFound(request.Id, page);

    // If this is a published page, create a new draft
    if (page.IsPublished)
    {
      var draft = new Page
      {
        Title = request.Title,
        SeoTitle = request.SeoTitle,
        MetaDescription = request.MetaDescription,
        Slug = request.Slug,
        LinkTarget = request.LinkTarget,
        Status = PageStatus.Draft,
        IsPublished = false,
        OriginalPageId = page.Id,
        Version = page.Version + 1
      };

      var components = request.Components.Select(c => new PageComponent
      {
        Type = c.Type,
        Configuration = c.Configuration,
        Order = c.Order,
        ComponentId = c.ComponentId
      }).ToList();

      draft.SetComponents(components);

      _context.Pages.Add(draft);
      await _context.SaveChangesAsync(cancellationToken);

      return ApiResponse<PageDto>.CreateSuccess(PageDto.FromEntity(draft));
    }

    // Update the existing draft
    page.Title = request.Title;
    page.SeoTitle = request.SeoTitle;
    page.MetaDescription = request.MetaDescription;
    page.Slug = request.Slug;
    page.LinkTarget = request.LinkTarget;

    var updatedComponents = request.Components.Select(c => new PageComponent
    {
      Type = c.Type,
      Configuration = c.Configuration,
      Order = c.Order,
      ComponentId = c.ComponentId
    }).ToList();

    page.SetComponents(updatedComponents);

    await _context.SaveChangesAsync(cancellationToken);

    return ApiResponse<PageDto>.CreateSuccess(PageDto.FromEntity(page));
  }
}
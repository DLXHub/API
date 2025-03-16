using API.Features.Pages.Models;
using API.Shared.Infrastructure;
using API.Shared.Models;
using Ardalis.GuardClauses;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Pages.Commands.PublishPage;

/// <summary>
/// Command to publish a page.
/// </summary>
public record PublishPageCommand : IRequest<ApiResponse<PageDto>>
{
  /// <summary>
  /// The ID of the page to publish.
  /// </summary>
  public Guid Id { get; init; }
}

/// <summary>
/// Validator for the PublishPageCommand.
/// </summary>
public class PublishPageCommandValidator : AbstractValidator<PublishPageCommand>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the PublishPageCommandValidator class.
  /// </summary>
  public PublishPageCommandValidator(ApplicationDbContext context)
  {
    _context = context;

    RuleFor(x => x.Id)
        .NotEmpty()
        .MustAsync(BeExistingDraft)
        .WithMessage("Page not found or is not a draft.");
  }

  private async Task<bool> BeExistingDraft(Guid id, CancellationToken cancellationToken)
  {
    var page = await _context.Pages.FindAsync(new object[] { id }, cancellationToken);
    return page != null && page.Status == PageStatus.Draft;
  }
}

/// <summary>
/// Handler for the PublishPageCommand.
/// </summary>
public class PublishPageCommandHandler : IRequestHandler<PublishPageCommand, ApiResponse<PageDto>>
{
  private readonly ApplicationDbContext _context;

  /// <summary>
  /// Initializes a new instance of the PublishPageCommandHandler class.
  /// </summary>
  public PublishPageCommandHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Handles the PublishPageCommand.
  /// </summary>
  public async Task<ApiResponse<PageDto>> Handle(PublishPageCommand request, CancellationToken cancellationToken)
  {
    var page = await _context.Pages
        .Include(p => p.OriginalPage)
        .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

    Guard.Against.NotFound(request.Id, page);
    Guard.Against.InvalidInput(page, nameof(page), p => p.Status == PageStatus.Draft, "Only draft pages can be published.");

    // If this is a draft of an existing page
    if (page.OriginalPage != null)
    {
      // Update the original page
      page.OriginalPage.Title = page.Title;
      page.OriginalPage.SeoTitle = page.SeoTitle;
      page.OriginalPage.MetaDescription = page.MetaDescription;
      page.OriginalPage.Slug = page.Slug;
      page.OriginalPage.LinkTarget = page.LinkTarget;
      page.OriginalPage.Components = page.Components;
      page.OriginalPage.Version = page.Version;
      page.OriginalPage.Status = PageStatus.Published;
      page.OriginalPage.IsPublished = true;
      page.OriginalPage.PublishedOn = DateTime.UtcNow;

      // Delete the draft
      _context.Pages.Remove(page);
    }
    else
    {
      // This is a new page being published for the first time
      page.Status = PageStatus.Published;
      page.IsPublished = true;
      page.PublishedOn = DateTime.UtcNow;
    }

    await _context.SaveChangesAsync(cancellationToken);

    var publishedPage = page.OriginalPage ?? page;
    return ApiResponse<PageDto>.CreateSuccess(PageDto.FromEntity(publishedPage));
  }
}
using System.Text.Json;

namespace API.Features.Pages.Models;

/// <summary>
/// Data Transfer Object for a page.
/// </summary>
public class PageDto
{
  /// <summary>
  /// The unique identifier of the page.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// The title of the page.
  /// </summary>
  public string Title { get; set; } = string.Empty;

  /// <summary>
  /// The SEO-friendly title of the page.
  /// </summary>
  public string? SeoTitle { get; set; }

  /// <summary>
  /// The SEO meta description of the page.
  /// </summary>
  public string? MetaDescription { get; set; }

  /// <summary>
  /// The URL slug for the page.
  /// </summary>
  public string Slug { get; set; } = string.Empty;

  /// <summary>
  /// The internal link target key used for routing and navigation.
  /// </summary>
  public string LinkTarget { get; set; } = string.Empty;

  /// <summary>
  /// The current status of the page.
  /// </summary>
  public PageStatus Status { get; set; }

  /// <summary>
  /// The components that make up the page layout.
  /// </summary>
  public List<PageComponentDto> Components { get; set; } = new();

  /// <summary>
  /// The version number of the page.
  /// </summary>
  public int Version { get; set; }

  /// <summary>
  /// Whether this page is the currently published version.
  /// </summary>
  public bool IsPublished { get; set; }

  /// <summary>
  /// The ID of the original page if this is a draft version.
  /// </summary>
  public Guid? OriginalPageId { get; set; }

  /// <summary>
  /// The date when this page was published.
  /// </summary>
  public DateTime? PublishedOn { get; set; }

  /// <summary>
  /// The username of the user who published this page.
  /// </summary>
  public string? PublishedByUsername { get; set; }

  /// <summary>
  /// The date when this page was created.
  /// </summary>
  public DateTime CreatedOn { get; set; }

  /// <summary>
  /// The username of the user who created this page.
  /// </summary>
  public string? CreatedByUsername { get; set; }

  /// <summary>
  /// The date when this page was last modified.
  /// </summary>
  public DateTime? ModifiedOn { get; set; }

  /// <summary>
  /// The username of the user who last modified this page.
  /// </summary>
  public string? ModifiedByUsername { get; set; }

  /// <summary>
  /// Maps a Page entity to a PageDto.
  /// </summary>
  /// <param name="page">The page entity to map.</param>
  /// <returns>A new PageDto instance.</returns>
  public static PageDto FromEntity(Page page)
  {
    return new PageDto
    {
      Id = page.Id,
      Title = page.Title,
      SeoTitle = page.SeoTitle,
      MetaDescription = page.MetaDescription,
      Slug = page.Slug,
      LinkTarget = page.LinkTarget,
      Status = page.Status,
      Components = page.GetComponents().Select(c => new PageComponentDto
      {
        Type = c.Type,
        Configuration = c.Configuration,
        Order = c.Order,
        ComponentId = c.ComponentId
      }).ToList(),
      Version = page.Version,
      IsPublished = page.IsPublished,
      OriginalPageId = page.OriginalPageId,
      PublishedOn = page.PublishedOn,
      PublishedByUsername = page.PublishedBy?.UserName,
      CreatedOn = page.CreatedOn,
      CreatedByUsername = page.CreatedBy?.UserName,
      ModifiedOn = page.ModifiedOn,
      ModifiedByUsername = page.ModifiedBy?.UserName
    };
  }
}

/// <summary>
/// Data Transfer Object for a page component.
/// </summary>
public class PageComponentDto
{
  /// <summary>
  /// The type of the component.
  /// </summary>
  public string Type { get; set; } = string.Empty;

  /// <summary>
  /// The configuration for the component.
  /// </summary>
  public JsonDocument Configuration { get; set; } = JsonDocument.Parse("{}");

  /// <summary>
  /// The order/position of the component on the page.
  /// </summary>
  public int Order { get; set; }

  /// <summary>
  /// The unique identifier for this component instance.
  /// </summary>
  public string ComponentId { get; set; } = string.Empty;
}
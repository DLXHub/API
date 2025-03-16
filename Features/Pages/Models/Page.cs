using System.Text.Json;
using API.Shared.Models;

namespace API.Features.Pages.Models;

/// <summary>
/// Represents a page in the system that can be built using the page builder.
/// </summary>
public class Page : BaseEntity
{
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
  /// The URL slug for the page (e.g., "about-us", "contact").
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
  /// The components and their configuration that make up the page layout.
  /// Stored as a JSON string containing an array of PageComponent objects.
  /// </summary>
  public string Components { get; set; } = "[]";

  /// <summary>
  /// The version number of the page, incremented with each update.
  /// </summary>
  public int Version { get; set; } = 1;

  /// <summary>
  /// Whether this page is the currently published version.
  /// </summary>
  public bool IsPublished { get; set; }

  /// <summary>
  /// The ID of the original page if this is a draft version.
  /// </summary>
  public Guid? OriginalPageId { get; set; }

  /// <summary>
  /// Reference to the original page if this is a draft version.
  /// </summary>
  public virtual Page? OriginalPage { get; set; }

  /// <summary>
  /// Collection of draft versions of this page.
  /// </summary>
  public virtual ICollection<Page> DraftVersions { get; set; } = new List<Page>();

  /// <summary>
  /// The date when this page was published.
  /// </summary>
  public DateTime? PublishedOn { get; set; }

  /// <summary>
  /// The ID of the user who published this page.
  /// </summary>
  public string? PublishedById { get; set; }

  /// <summary>
  /// Reference to the user who published this page.
  /// </summary>
  public virtual ApplicationUser? PublishedBy { get; set; }

  /// <summary>
  /// Gets the page components as a strongly-typed list.
  /// </summary>
  /// <returns>The list of page components.</returns>
  public List<PageComponent> GetComponents()
  {
    return JsonSerializer.Deserialize<List<PageComponent>>(Components) ?? new List<PageComponent>();
  }

  /// <summary>
  /// Sets the page components from a list.
  /// </summary>
  /// <param name="components">The list of components to set.</param>
  public void SetComponents(List<PageComponent> components)
  {
    Components = JsonSerializer.Serialize(components);
  }
}

/// <summary>
/// Represents the status of a page.
/// </summary>
public enum PageStatus
{
  /// <summary>
  /// The page is a draft and not visible to the public.
  /// </summary>
  Draft,

  /// <summary>
  /// The page is published and visible to the public.
  /// </summary>
  Published,

  /// <summary>
  /// The page is archived and not visible to the public.
  /// </summary>
  Archived
}

/// <summary>
/// Represents a component on a page with its configuration.
/// </summary>
public class PageComponent
{
  /// <summary>
  /// The type of the component (e.g., "Hero", "TextBlock", "ImageGallery").
  /// </summary>
  public string Type { get; set; } = string.Empty;

  /// <summary>
  /// The configuration for the component, stored as a JSON object.
  /// </summary>
  public JsonDocument Configuration { get; set; } = JsonDocument.Parse("{}");

  /// <summary>
  /// The order/position of the component on the page.
  /// </summary>
  public int Order { get; set; }

  /// <summary>
  /// The unique identifier for this component instance.
  /// </summary>
  public string ComponentId { get; set; } = Guid.NewGuid().ToString();
}
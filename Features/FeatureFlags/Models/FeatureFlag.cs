using API.Shared.Models;

namespace API.Features.FeatureFlags.Models;

public class FeatureFlag : BaseEntity
{
  public string Key { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string Category { get; set; } = string.Empty;
  public bool IsEnabled { get; set; }
  public Dictionary<string, object>? Configuration { get; set; }
  public string? ClientKey { get; set; } // Für Client-Side Feature Flags
}
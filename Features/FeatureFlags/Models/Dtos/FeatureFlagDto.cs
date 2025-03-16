namespace API.Features.FeatureFlags.Models.Dtos;

public record FeatureFlagDto
{
  public Guid Id { get; init; }
  public string Key { get; init; } = string.Empty;
  public string Description { get; init; } = string.Empty;
  public string Category { get; init; } = string.Empty;
  public bool IsEnabled { get; init; }
  public Dictionary<string, object>? Configuration { get; init; }
  public string? ClientKey { get; init; }

  public static FeatureFlagDto FromEntity(FeatureFlag entity) => new()
  {
    Id = entity.Id,
    Key = entity.Key,
    Description = entity.Description,
    Category = entity.Category,
    IsEnabled = entity.IsEnabled,
    Configuration = entity.Configuration,
    ClientKey = entity.ClientKey
  };
}

public record CreateFeatureFlagDto
{
  public string Key { get; init; } = string.Empty;
  public string Description { get; init; } = string.Empty;
  public string Category { get; init; } = string.Empty;
  public bool IsEnabled { get; init; }
  public Dictionary<string, object>? Configuration { get; init; }
  public string? ClientKey { get; init; }
}

public record UpdateFeatureFlagDto
{
  public string Description { get; init; } = string.Empty;
  public string Category { get; init; } = string.Empty;
  public bool IsEnabled { get; init; }
  public Dictionary<string, object>? Configuration { get; init; }
  public string? ClientKey { get; init; }
}
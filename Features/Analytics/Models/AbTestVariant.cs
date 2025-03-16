namespace Features.Analytics.Models
{
  public class AbTestVariant
  {
    public string TestKey { get; set; } = string.Empty;
    public string VariantKey { get; set; } = string.Empty;
    public Dictionary<string, object>? Parameters { get; set; }
  }
}
using System.Collections.Generic;

namespace Features.Analytics.Models.Dtos
{
  public class SeoAnalysisResult
  {
    public string Path { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? MetaDescription { get; set; }
    public Dictionary<string, string> HeadingStructure { get; set; } = new();
    public List<string> MissingAltTags { get; set; } = new();
    public List<string> BrokenLinks { get; set; } = new();
    public int WordCount { get; set; }
    public double ReadabilityScore { get; set; }
    public Dictionary<string, List<string>> KeywordDensity { get; set; } = new();
    public bool HasSitemap { get; set; }
    public bool HasRobotsTxt { get; set; }
    public Dictionary<string, string> SeoIssues { get; set; } = new();
  }
}
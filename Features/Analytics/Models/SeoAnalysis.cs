using System;
using System.Collections.Generic;
using API.Shared.Models;

namespace Features.Analytics.Models
{
  public class SeoAnalysis : BaseEntity
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
using System;
using System.Collections.Generic;

namespace Features.Analytics.Models
{
  public class SeoReportDto
  {
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalPageViews { get; set; }
    public int UniqueVisitors { get; set; }
    public Dictionary<string, int> TopPages { get; set; } = new();
    public Dictionary<string, int> TopReferrers { get; set; } = new();
    public Dictionary<string, double> BounceRates { get; set; } = new();
    public Dictionary<string, TimeSpan> AverageTimeOnPage { get; set; } = new();
  }
}
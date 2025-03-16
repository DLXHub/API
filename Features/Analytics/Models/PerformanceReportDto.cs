using System;
using System.Collections.Generic;

namespace Features.Analytics.Models
{
  public class PerformanceReportDto
  {
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double AverageLoadTime { get; set; }
    public double AverageFirstContentfulPaint { get; set; }
    public double AverageLargestContentfulPaint { get; set; }
    public double AverageFirstInputDelay { get; set; }
    public double AverageCumulativeLayoutShift { get; set; }
    public List<PagePerformanceMetric> TopSlowPages { get; set; } = new();
    public Dictionary<string, double> PerformanceByDevice { get; set; } = new();
  }

  public class PagePerformanceMetric
  {
    public string Path { get; set; } = string.Empty;
    public double AverageLoadTime { get; set; }
    public int SampleSize { get; set; }
  }
}
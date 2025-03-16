namespace Features.Analytics.Models.Dtos
{
  public class PerformanceMetricDto
  {
    public string Path { get; set; } = string.Empty;
    public double LoadTime { get; set; }
    public double FirstContentfulPaint { get; set; }
    public double LargestContentfulPaint { get; set; }
    public double FirstInputDelay { get; set; }
    public double CumulativeLayoutShift { get; set; }
    public string? UserAgent { get; set; }
    public string? DeviceType { get; set; }
    public string? NetworkInfo { get; set; }
  }
}
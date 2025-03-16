using System;
using System.Collections.Generic;
using System.Text.Json;
using API.Shared.Models;

namespace API.Features.Analytics.Models
{
  public class AbTest : BaseEntity
  {
    public string Name { get; set; } = string.Empty;
    public string FeatureKey { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Dictionary<string, double> VariantDistribution { get; set; } = new();
    public Dictionary<string, JsonDocument> VariantConfigurations { get; set; } = new();
    public virtual ICollection<AbTestMetric> Metrics { get; set; } = new List<AbTestMetric>();
  }

  public class AbTestMetric : BaseEntity
  {
    public string Variant { get; set; } = string.Empty;
    public string MetricName { get; set; } = string.Empty;
    public double Value { get; set; }
    public int SampleSize { get; set; }
    public Guid AbTestId { get; set; }
    public virtual AbTest AbTest { get; set; } = null!;
  }
}
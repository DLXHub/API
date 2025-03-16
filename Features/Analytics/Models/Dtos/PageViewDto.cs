using System;
using System.Collections.Generic;

namespace Features.Analytics.Models.Dtos
{
  public class PageViewDto
  {
    public string Path { get; set; } = string.Empty;
    public string? ReferrerUrl { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
    public string? SessionId { get; set; }
    public string? UserId { get; set; }
    public TimeSpan Duration { get; set; }
    public Dictionary<string, string> CustomData { get; set; } = new();
  }
}
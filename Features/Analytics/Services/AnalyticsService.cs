using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Features.Analytics.Models;
using API.Shared.Infrastructure;
using API.Shared.Infrastructure.Caching;
using Features.Analytics.Models;
using Features.Analytics.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Features.Analytics.Services
{
  public interface IAnalyticsService
  {
    Task TrackPageViewAsync(PageViewDto pageView);
    Task<Features.Analytics.Models.Dtos.AbTestVariant> GetAbTestVariantAsync(string testKey, string sessionId);
    Task TrackPerformanceMetricsAsync(PerformanceMetricDto metrics);
    Task<SeoAnalysisResult> AnalyzePageSeoAsync(string path);
  }

  public class AnalyticsService : IAnalyticsService
  {
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(
        ApplicationDbContext context,
        ICacheService cache,
        ILogger<AnalyticsService> logger)
    {
      _context = context;
      _cache = cache;
      _logger = logger;
    }

    public async Task TrackPageViewAsync(PageViewDto pageView)
    {
      var entity = new PageView
      {
        Path = pageView.Path,
        ReferrerUrl = pageView.ReferrerUrl,
        UserAgent = pageView.UserAgent,
        IpAddress = pageView.IpAddress,
        SessionId = pageView.SessionId,
        UserId = pageView.UserId,
        Duration = pageView.Duration,
        CustomData = pageView.CustomData
      };

      _context.PageViews.Add(entity);
      await _context.SaveChangesAsync();
    }

    public async Task<Features.Analytics.Models.Dtos.AbTestVariant> GetAbTestVariantAsync(string testKey, string sessionId)
    {
      var cacheKey = $"abtest:{testKey}:{sessionId}";
      var variant = await _cache.GetAsync<string>(cacheKey);

      if (variant != null)
      {
        return new Features.Analytics.Models.Dtos.AbTestVariant { TestKey = testKey, Variant = variant };
      }

      var test = await _context.AbTests
          .FirstOrDefaultAsync(t => t.FeatureKey == testKey && t.IsActive);

      if (test == null)
      {
        return new Features.Analytics.Models.Dtos.AbTestVariant { TestKey = testKey, Variant = "default" };
      }

      variant = AssignVariant(test.VariantDistribution);
      await _cache.SetAsync(cacheKey, variant, TimeSpan.FromDays(30));

      return new Features.Analytics.Models.Dtos.AbTestVariant { TestKey = testKey, Variant = variant };
    }

    private string AssignVariant(Dictionary<string, double> distribution)
    {
      var random = new Random();
      var value = random.NextDouble();
      var cumulative = 0.0;

      foreach (var variant in distribution)
      {
        cumulative += variant.Value;
        if (value <= cumulative)
        {
          return variant.Key;
        }
      }

      return distribution.First().Key;
    }

    public async Task TrackPerformanceMetricsAsync(PerformanceMetricDto metrics)
    {
      var entity = new PerformanceMetric
      {
        Path = metrics.Path,
        LoadTime = metrics.LoadTime,
        FirstContentfulPaint = metrics.FirstContentfulPaint,
        LargestContentfulPaint = metrics.LargestContentfulPaint,
        FirstInputDelay = metrics.FirstInputDelay,
        CumulativeLayoutShift = metrics.CumulativeLayoutShift,
        UserAgent = metrics.UserAgent,
        DeviceType = metrics.DeviceType,
        NetworkInfo = metrics.NetworkInfo
      };

      _context.Set<PerformanceMetric>().Add(entity);
      await _context.SaveChangesAsync();
    }

    public async Task<SeoAnalysisResult> AnalyzePageSeoAsync(string path)
    {
      var analysis = await _context.Set<SeoAnalysis>()
          .FirstOrDefaultAsync(a => a.Path == path);

      if (analysis == null)
        return new SeoAnalysisResult { Path = path };

      return new SeoAnalysisResult
      {
        Path = analysis.Path,
        Title = analysis.Title,
        MetaDescription = analysis.MetaDescription,
        HeadingStructure = analysis.HeadingStructure,
        MissingAltTags = analysis.MissingAltTags,
        BrokenLinks = analysis.BrokenLinks,
        WordCount = analysis.WordCount,
        ReadabilityScore = analysis.ReadabilityScore,
        KeywordDensity = analysis.KeywordDensity,
        HasSitemap = analysis.HasSitemap,
        HasRobotsTxt = analysis.HasRobotsTxt,
        SeoIssues = analysis.SeoIssues
      };
    }
  }
}
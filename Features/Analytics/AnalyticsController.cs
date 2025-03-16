using API.Shared.Models;
using Features.Analytics.Models;
using Features.Analytics.Models.Dtos;
using Features.Analytics.Queries.GetPerformanceReport;
using Features.Analytics.Queries.GetSeoReport;
using Features.Analytics.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Features.Analytics
{
  [ApiController]
  [Route("api/analytics")]
  public class AnalyticsController : ControllerBase
  {
    private readonly IAnalyticsService _analyticsService;
    private readonly IMediator _mediator;

    public AnalyticsController(IAnalyticsService analyticsService, IMediator mediator)
    {
      _analyticsService = analyticsService;
      _mediator = mediator;
    }

    [HttpPost("pageview")]
    public async Task<ActionResult> TrackPageView(PageViewDto pageView)
    {
      await _analyticsService.TrackPageViewAsync(pageView);
      return Ok();
    }

    [HttpGet("abtest/{testKey}")]
    public async Task<ActionResult<Features.Analytics.Models.Dtos.AbTestVariant>> GetAbTestVariant(string testKey)
    {
      var sessionId = HttpContext.Session.Id;
      var variant = await _analyticsService.GetAbTestVariantAsync(testKey, sessionId);
      return Ok(variant);
    }

    [HttpGet("reports/performance")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<PerformanceReportDto>>> GetPerformanceReport(
        [FromQuery] GetPerformanceReportQuery query)
    {
      var result = await _mediator.Send(query);
      return Ok(result);
    }

    [HttpGet("reports/seo")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<SeoReportDto>>> GetSeoReport(
        [FromQuery] GetSeoReportQuery query)
    {
      var result = await _mediator.Send(query);
      return Ok(result);
    }
  }
}
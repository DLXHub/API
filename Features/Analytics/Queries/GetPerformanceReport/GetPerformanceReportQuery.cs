using API.Shared.Infrastructure;
using API.Shared.Models;
using Features.Analytics.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Features.Analytics.Queries.GetPerformanceReport
{
  public class GetPerformanceReportQuery : IRequest<ApiResponse<PerformanceReportDto>>
  {
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? TopPagesLimit { get; set; }
    public string? DeviceType { get; set; }
  }

  public class GetPerformanceReportQueryHandler : IRequestHandler<GetPerformanceReportQuery, ApiResponse<PerformanceReportDto>>
  {
    private readonly ApplicationDbContext _context;

    public GetPerformanceReportQueryHandler(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<ApiResponse<PerformanceReportDto>> Handle(
        GetPerformanceReportQuery request,
        CancellationToken cancellationToken)
    {
      var metrics = await _context.PerformanceMetrics
          .Where(m => m.CreatedOn >= request.StartDate &&
                     m.CreatedOn <= request.EndDate &&
                     (request.DeviceType == null || m.DeviceType == request.DeviceType))
          .ToListAsync(cancellationToken);

      var report = new PerformanceReportDto
      {
        StartDate = request.StartDate,
        EndDate = request.EndDate,
        AverageLoadTime = metrics.Average(m => m.LoadTime),
        AverageFirstContentfulPaint = metrics.Average(m => m.FirstContentfulPaint),
        AverageLargestContentfulPaint = metrics.Average(m => m.LargestContentfulPaint),
        AverageFirstInputDelay = metrics.Average(m => m.FirstInputDelay),
        AverageCumulativeLayoutShift = metrics.Average(m => m.CumulativeLayoutShift),
        PerformanceByDevice = metrics.GroupBy(m => m.DeviceType ?? "unknown")
                                   .ToDictionary(g => g.Key, g => g.Average(m => m.LoadTime))
      };

      return ApiResponse<PerformanceReportDto>.CreateSuccess(report);
    }
  }
}
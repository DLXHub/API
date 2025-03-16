using System;
using API.Shared.Models;
using Features.Analytics.Models;
using MediatR;

namespace Features.Analytics.Queries.GetSeoReport
{
  public class GetSeoReportQuery : IRequest<ApiResponse<SeoReportDto>>
  {
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? TopPagesLimit { get; set; }
    public int? TopReferrersLimit { get; set; }
  }
}
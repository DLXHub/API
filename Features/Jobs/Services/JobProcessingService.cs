using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Features.Jobs.Services;

public class JobProcessingService : BackgroundService
{
  private readonly IJobService _jobService;
  private readonly ILogger<JobProcessingService> _logger;
  private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(30);

  public JobProcessingService(
      IJobService jobService,
      ILogger<JobProcessingService> logger)
  {
    _jobService = jobService;
    _logger = logger;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      try
      {
        await _jobService.ProcessScheduledJobsAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error processing scheduled jobs");
      }

      await Task.Delay(_checkInterval, stoppingToken);
    }
  }
}
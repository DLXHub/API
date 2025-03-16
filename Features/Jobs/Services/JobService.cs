using System.Text.Json;
using API.Features.Jobs.JobHandlers;
using API.Features.Jobs.Models;
using API.Features.Jobs.Models.Dtos;
using API.Shared.Infrastructure;
using Cronos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace API.Features.Jobs.Services;

public interface IJobService
{
  Task<Job> CreateJobAsync(CreateJobDto dto);
  Task<Job?> GetJobAsync(Guid id);
  Task<IEnumerable<Job>> GetJobsAsync(JobStatus? status = null);
  Task<Job> StartJobAsync(Guid id);
  Task<Job> CancelJobAsync(Guid id);
  Task ProcessScheduledJobsAsync();
}

public class JobService : IJobService
{
  private readonly ApplicationDbContext _context;
  private readonly ILogger<JobService> _logger;
  private readonly IServiceProvider _serviceProvider;
  private readonly IDictionary<JobType, Type> _jobHandlers;

  public JobService(
      ApplicationDbContext context,
      ILogger<JobService> logger,
      IServiceProvider serviceProvider)
  {
    _context = context;
    _logger = logger;
    _serviceProvider = serviceProvider;

    _jobHandlers = new Dictionary<JobType, Type>
    {
      { JobType.GenerateSitemap, typeof(GenerateSitemapJobHandler) },
      { JobType.CleanupTempFiles, typeof(CleanupTempFilesJobHandler) },
      { JobType.UpdateSearchIndex, typeof(UpdateSearchIndexJobHandler) }
    };
  }

  public async Task<Job> CreateJobAsync(CreateJobDto dto)
  {
    var job = new Job
    {
      Name = dto.Name,
      Description = dto.Description,
      Type = dto.Type,
      Status = dto.StartImmediately ? JobStatus.Pending : JobStatus.Cancelled,
      ScheduleType = dto.ScheduleType,
      CronExpression = dto.CronExpression,
      Parameters = dto.Parameters,
      IsEnabled = true
    };

    if (dto.ScheduleType == JobScheduleType.Recurring && !string.IsNullOrEmpty(dto.CronExpression))
    {
      job.NextRun = GetNextRunTime(dto.CronExpression);
    }
    else if (dto.StartImmediately)
    {
      job.NextRun = DateTime.UtcNow;
    }

    _context.Jobs.Add(job);
    await _context.SaveChangesAsync();

    return job;
  }

  public async Task<Job?> GetJobAsync(Guid id)
  {
    return await _context.Jobs.FindAsync(id);
  }

  public async Task<IEnumerable<Job>> GetJobsAsync(JobStatus? status = null)
  {
    var query = _context.Jobs.AsQueryable();

    if (status.HasValue)
    {
      query = query.Where(j => j.Status == status.Value);
    }

    return await query.OrderByDescending(j => j.CreatedOn).ToListAsync();
  }

  public async Task<Job> StartJobAsync(Guid id)
  {
    var job = await _context.Jobs.FindAsync(id);
    if (job == null) throw new ArgumentException("Job not found", nameof(id));

    if (job.Status == JobStatus.Running)
      throw new InvalidOperationException("Job is already running");

    job.Status = JobStatus.Pending;
    job.NextRun = DateTime.UtcNow;
    await _context.SaveChangesAsync();

    return job;
  }

  public async Task<Job> CancelJobAsync(Guid id)
  {
    var job = await _context.Jobs.FindAsync(id);
    if (job == null) throw new ArgumentException("Job not found", nameof(id));

    if (job.Status == JobStatus.Running)
      throw new InvalidOperationException("Cannot cancel a running job");

    job.Status = JobStatus.Cancelled;
    job.NextRun = null;
    await _context.SaveChangesAsync();

    return job;
  }

  public async Task ProcessScheduledJobsAsync()
  {
    var pendingJobs = await _context.Jobs
        .Where(j => j.Status == JobStatus.Pending
            && j.IsEnabled
            && j.NextRun <= DateTime.UtcNow)
        .ToListAsync();

    foreach (var job in pendingJobs)
    {
      try
      {
        job.Status = JobStatus.Running;
        job.LastRun = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await ExecuteJobAsync(job);

        job.Status = JobStatus.Completed;
        if (job.ScheduleType == JobScheduleType.Recurring && !string.IsNullOrEmpty(job.CronExpression))
        {
          job.NextRun = GetNextRunTime(job.CronExpression);
          job.Status = JobStatus.Pending;
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error executing job {JobId}", job.Id);
        job.Status = JobStatus.Failed;
        job.ErrorMessage = ex.Message;
      }
      finally
      {
        await _context.SaveChangesAsync();
      }
    }
  }

  private async Task ExecuteJobAsync(Job job)
  {
    if (!_jobHandlers.TryGetValue(job.Type, out var handlerType))
    {
      throw new ArgumentException($"Unknown job type: {job.Type}");
    }

    var handler = ActivatorUtilities.CreateInstance(_serviceProvider, handlerType) as IJobHandler;
    if (handler == null)
    {
      throw new InvalidOperationException($"Could not create handler for job type: {job.Type}");
    }

    await handler.ExecuteAsync(job);
  }

  private DateTime? GetNextRunTime(string cronExpression)
  {
    try
    {
      var cron = CronExpression.Parse(cronExpression);
      return cron.GetNextOccurrence(DateTime.UtcNow);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error parsing cron expression: {CronExpression}", cronExpression);
      return null;
    }
  }
}

public class SearchDocument
{
  public Guid Id { get; set; }
  public SearchDocumentType Type { get; set; }
  public string Title { get; set; } = string.Empty;
  public string? Description { get; set; }
  public List<string> Keywords { get; set; } = new();
  public string Url { get; set; } = string.Empty;
  public DateTime LastModified { get; set; }
}

public enum SearchDocumentType
{
  Movie,
  TvShow,
  Page
}
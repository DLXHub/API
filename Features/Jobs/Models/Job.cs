using API.Shared.Models;

namespace API.Features.Jobs.Models;

public class Job : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public JobType Type { get; set; }
  public JobStatus Status { get; set; }
  public JobScheduleType ScheduleType { get; set; }
  public string? CronExpression { get; set; }
  public DateTime? LastRun { get; set; }
  public DateTime? NextRun { get; set; }
  public bool IsEnabled { get; set; }
  public string? ErrorMessage { get; set; }
  public Dictionary<string, string>? Parameters { get; set; }
}

public enum JobType
{
  GenerateSitemap,
  CleanupTempFiles,
  UpdateSearchIndex,
  // Weitere Job-Typen hier hinzufügen
}

public enum JobStatus
{
  Pending,
  Running,
  Completed,
  Failed,
  Cancelled
}

public enum JobScheduleType
{
  RunOnce,
  Recurring
}
namespace API.Features.Jobs.Models.Dtos;

public record CreateJobDto
{
  public string Name { get; init; } = string.Empty;
  public string Description { get; init; } = string.Empty;
  public JobType Type { get; init; }
  public JobScheduleType ScheduleType { get; init; }
  public string? CronExpression { get; init; }
  public Dictionary<string, string>? Parameters { get; init; }
  public bool StartImmediately { get; init; }
}
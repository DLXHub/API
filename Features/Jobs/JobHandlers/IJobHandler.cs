using API.Features.Jobs.Models;

namespace API.Features.Jobs.JobHandlers;

public interface IJobHandler
{
  Task ExecuteAsync(Job job);
}
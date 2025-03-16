using API.Features.Jobs.Models;
using API.Features.Jobs.Models.Dtos;
using API.Features.Jobs.Services;
using API.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Jobs;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class JobsController : ControllerBase
{
  private readonly IJobService _jobService;

  public JobsController(IJobService jobService)
  {
    _jobService = jobService;
  }

  [HttpPost]
  public async Task<ActionResult<ApiResponse<Job>>> CreateJob(CreateJobDto dto)
  {
    var job = await _jobService.CreateJobAsync(dto);
    return Ok(ApiResponse<Job>.CreateSuccess(job));
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ApiResponse<Job>>> GetJob(Guid id)
  {
    var job = await _jobService.GetJobAsync(id);
    if (job == null)
      return NotFound(ApiResponse<Job>.CreateError("Job not found"));

    return Ok(ApiResponse<Job>.CreateSuccess(job));
  }

  [HttpGet]
  public async Task<ActionResult<ApiResponse<IEnumerable<Job>>>> GetJobs([FromQuery] JobStatus? status)
  {
    var jobs = await _jobService.GetJobsAsync(status);
    return Ok(ApiResponse<IEnumerable<Job>>.CreateSuccess(jobs));
  }

  [HttpPost("{id}/start")]
  public async Task<ActionResult<ApiResponse<Job>>> StartJob(Guid id)
  {
    try
    {
      var job = await _jobService.StartJobAsync(id);
      return Ok(ApiResponse<Job>.CreateSuccess(job));
    }
    catch (ArgumentException ex)
    {
      return NotFound(ApiResponse<Job>.CreateError(ex.Message));
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(ApiResponse<Job>.CreateError(ex.Message));
    }
  }

  [HttpPost("{id}/cancel")]
  public async Task<ActionResult<ApiResponse<Job>>> CancelJob(Guid id)
  {
    try
    {
      var job = await _jobService.CancelJobAsync(id);
      return Ok(ApiResponse<Job>.CreateSuccess(job));
    }
    catch (ArgumentException ex)
    {
      return NotFound(ApiResponse<Job>.CreateError(ex.Message));
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(ApiResponse<Job>.CreateError(ex.Message));
    }
  }
}
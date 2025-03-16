using API.Features.FeatureFlags.Models;
using API.Features.FeatureFlags.Models.Dtos;
using API.Features.FeatureFlags.Services;
using API.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.FeatureFlags;

[ApiController]
[Route("api/[controller]")]
public class FeatureFlagsController : ControllerBase
{
  private readonly IFeatureFlagService _featureFlagService;

  public FeatureFlagsController(IFeatureFlagService featureFlagService)
  {
    _featureFlagService = featureFlagService;
  }

  [HttpGet("client")]
  public async Task<ActionResult<ApiResponse<Dictionary<string, bool>>>> GetClientFlags()
  {
    var flags = await _featureFlagService.GetClientFlagsAsync();
    return Ok(ApiResponse<Dictionary<string, bool>>.CreateSuccess(flags));
  }

  [Authorize(Roles = "Admin")]
  [HttpGet]
  public async Task<ActionResult<ApiResponse<IEnumerable<FeatureFlagDto>>>> GetAllFlags()
  {
    var flags = await _featureFlagService.GetAllFlagsAsync();
    var dtos = flags.Select(FeatureFlagDto.FromEntity);
    return Ok(ApiResponse<IEnumerable<FeatureFlagDto>>.CreateSuccess(dtos));
  }

  [Authorize(Roles = "Admin")]
  [HttpGet("{key}")]
  public async Task<ActionResult<ApiResponse<FeatureFlagDto>>> GetFlag(string key)
  {
    var flag = await _featureFlagService.GetFlagAsync(key);
    if (flag == null)
      return NotFound(ApiResponse<FeatureFlagDto>.CreateError("Feature flag not found"));

    return Ok(ApiResponse<FeatureFlagDto>.CreateSuccess(FeatureFlagDto.FromEntity(flag)));
  }

  [Authorize(Roles = "Admin")]
  [HttpPost]
  public async Task<ActionResult<ApiResponse<FeatureFlagDto>>> CreateFlag(CreateFeatureFlagDto dto)
  {
    var flag = new FeatureFlag
    {
      Key = dto.Key,
      Description = dto.Description,
      Category = dto.Category,
      IsEnabled = dto.IsEnabled,
      Configuration = dto.Configuration,
      ClientKey = dto.ClientKey
    };

    var created = await _featureFlagService.CreateFlagAsync(flag);
    return Ok(ApiResponse<FeatureFlagDto>.CreateSuccess(FeatureFlagDto.FromEntity(created)));
  }

  [Authorize(Roles = "Admin")]
  [HttpPut("{key}")]
  public async Task<ActionResult<ApiResponse<FeatureFlagDto>>> UpdateFlag(string key, UpdateFeatureFlagDto dto)
  {
    var updated = await _featureFlagService.UpdateFlagAsync(key, flag =>
    {
      flag.Description = dto.Description;
      flag.Category = dto.Category;
      flag.IsEnabled = dto.IsEnabled;
      flag.Configuration = dto.Configuration;
      flag.ClientKey = dto.ClientKey;
    });

    if (updated == null)
      return NotFound(ApiResponse<FeatureFlagDto>.CreateError("Feature flag not found"));

    return Ok(ApiResponse<FeatureFlagDto>.CreateSuccess(FeatureFlagDto.FromEntity(updated)));
  }

  [Authorize(Roles = "Admin")]
  [HttpDelete("{key}")]
  public async Task<ActionResult<ApiResponse<bool>>> DeleteFlag(string key)
  {
    var result = await _featureFlagService.DeleteFlagAsync(key);
    if (!result)
      return NotFound(ApiResponse<bool>.CreateError("Feature flag not found"));

    return Ok(ApiResponse<bool>.CreateSuccess(true));
  }
}
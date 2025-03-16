using API.Features.Languages.Models;
using API.Features.Languages.Models.Dtos;
using API.Features.Languages.Services;
using API.Shared.Models;
using API.Shared.Models.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Languages.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LanguagesController : ControllerBase
{
  private readonly ILanguageService _languageService;

  public LanguagesController(ILanguageService languageService)
  {
    _languageService = languageService;
  }

  [HttpGet]
  public async Task<ActionResult<ApiResponse<IEnumerable<Language>>>> GetLanguages()
  {
    var languages = await _languageService.GetLanguagesAsync();
    return Ok(ApiResponse<IEnumerable<Language>>.CreateSuccess(languages));
  }

  [HttpGet("{isoCode}")]
  public async Task<ActionResult<ApiResponse<Language>>> GetLanguage(string isoCode)
  {
    var language = await _languageService.GetLanguageAsync(isoCode);
    if (language == null)
    {
      return NotFound(ApiResponse<Language>.CreateError($"Language with ISO code '{isoCode}' not found"));
    }

    return Ok(ApiResponse<Language>.CreateSuccess(language));
  }

  [HttpPost]
  [Authorize(Roles = "Admin")]
  public async Task<ActionResult<ApiResponse<Language>>> CreateLanguage(CreateLanguageDto dto)
  {
    try
    {
      var language = await _languageService.CreateLanguageAsync(dto);
      return CreatedAtAction(
          nameof(GetLanguage),
          new { isoCode = language.IsoCode },
          ApiResponse<Language>.CreateSuccess(language));
    }
    catch (Exception ex)
    {
      return BadRequest(ApiResponse<Language>.CreateError(ex.Message));
    }
  }

  [HttpPut("{isoCode}")]
  [Authorize(Roles = "Admin")]
  public async Task<ActionResult<ApiResponse<Language>>> UpdateLanguage(
      string isoCode,
      UpdateLanguageDto dto)
  {
    try
    {
      var language = await _languageService.UpdateLanguageAsync(isoCode, dto);
      return Ok(ApiResponse<Language>.CreateSuccess(language));
    }
    catch (NotFoundException ex)
    {
      return NotFound(ApiResponse<Language>.CreateError(ex.Message));
    }
    catch (Exception ex)
    {
      return BadRequest(ApiResponse<Language>.CreateError(ex.Message));
    }
  }

  [HttpDelete("{isoCode}")]
  [Authorize(Roles = "Admin")]
  public async Task<ActionResult<ApiResponse<bool>>> DeleteLanguage(string isoCode)
  {
    try
    {
      await _languageService.DeleteLanguageAsync(isoCode);
      return Ok(ApiResponse<bool>.CreateSuccess(true));
    }
    catch (NotFoundException ex)
    {
      return NotFound(ApiResponse<bool>.CreateError(ex.Message));
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(ApiResponse<bool>.CreateError(ex.Message));
    }
  }

  [HttpPost("{isoCode}/set-default")]
  [Authorize(Roles = "Admin")]
  public async Task<ActionResult<ApiResponse<Language>>> SetDefaultLanguage(string isoCode)
  {
    try
    {
      var language = await _languageService.SetDefaultLanguageAsync(isoCode);
      return Ok(ApiResponse<Language>.CreateSuccess(language));
    }
    catch (NotFoundException ex)
    {
      return NotFound(ApiResponse<Language>.CreateError(ex.Message));
    }
    catch (Exception ex)
    {
      return BadRequest(ApiResponse<Language>.CreateError(ex.Message));
    }
  }
}
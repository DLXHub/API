using API.Features.Languages.Models;
using API.Features.Languages.Models.Dtos;
using API.Shared.Infrastructure;
using API.Shared.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Languages.Services;

public interface ILanguageService
{
  Task<IEnumerable<Language>> GetLanguagesAsync();
  Task<Language?> GetLanguageAsync(string isoCode);
  Task<Language> CreateLanguageAsync(CreateLanguageDto dto);
  Task<Language> UpdateLanguageAsync(string isoCode, UpdateLanguageDto dto);
  Task DeleteLanguageAsync(string isoCode);
  Task<Language> SetDefaultLanguageAsync(string isoCode);
  Task<bool> IsLanguageActiveAsync(string isoCode);
}

public class LanguageService : ILanguageService
{
  private readonly ApplicationDbContext _context;
  private readonly ILogger<LanguageService> _logger;

  public LanguageService(
      ApplicationDbContext context,
      ILogger<LanguageService> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task<IEnumerable<Language>> GetLanguagesAsync()
  {
    return await _context.Set<Language>()
        .Where(l => l.IsActive)
        .OrderBy(l => l.Name)
        .ToListAsync();
  }

  public async Task<Language?> GetLanguageAsync(string isoCode)
  {
    return await _context.Set<Language>()
        .FirstOrDefaultAsync(l => l.IsoCode == isoCode);
  }

  public async Task<Language> CreateLanguageAsync(CreateLanguageDto dto)
  {
    var language = new Language
    {
      IsoCode = dto.IsoCode.ToLower(),
      Name = dto.Name,
      IsActive = true,
      FlagIcon = dto.FlagIcon
    };

    _context.Set<Language>().Add(language);
    await _context.SaveChangesAsync();

    return language;
  }

  public async Task<Language> UpdateLanguageAsync(string isoCode, UpdateLanguageDto dto)
  {
    var language = await GetLanguageAsync(isoCode);
    if (language == null)
    {
      throw new NotFoundException($"Language with ISO code '{isoCode}' not found");
    }

    language.Name = dto.Name;
    language.IsActive = dto.IsActive;
    language.FlagIcon = dto.FlagIcon;

    await _context.SaveChangesAsync();
    return language;
  }

  public async Task DeleteLanguageAsync(string isoCode)
  {
    var language = await GetLanguageAsync(isoCode);
    if (language == null)
    {
      throw new NotFoundException($"Language with ISO code '{isoCode}' not found");
    }

    if (language.IsDefault)
    {
      throw new InvalidOperationException("Cannot delete the default language");
    }

    _context.Set<Language>().Remove(language);
    await _context.SaveChangesAsync();
  }

  public async Task<Language> SetDefaultLanguageAsync(string isoCode)
  {
    var language = await GetLanguageAsync(isoCode);
    if (language == null)
    {
      throw new NotFoundException($"Language with ISO code '{isoCode}' not found");
    }

    // Reset all languages to non-default
    await _context.Set<Language>()
        .Where(l => l.IsDefault)
        .ExecuteUpdateAsync(s => s.SetProperty(l => l.IsDefault, false));

    // Set new default language
    language.IsDefault = true;
    await _context.SaveChangesAsync();

    return language;
  }

  public async Task<bool> IsLanguageActiveAsync(string isoCode)
  {
    var language = await GetLanguageAsync(isoCode);
    return language?.IsActive ?? false;
  }
}
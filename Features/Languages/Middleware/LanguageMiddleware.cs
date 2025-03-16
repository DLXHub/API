using API.Features.Languages.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace API.Features.Languages.Middleware;

public class LanguageMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<LanguageMiddleware> _logger;
  private readonly IServiceProvider _serviceProvider;

  public LanguageMiddleware(
      RequestDelegate next,
      ILogger<LanguageMiddleware> logger,
      IServiceProvider serviceProvider)
  {
    _next = next;
    _logger = logger;
    _serviceProvider = serviceProvider;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    var path = context.Request.Path.Value;
    if (path == null)
    {
      await _next(context);
      return;
    }

    // Skip language check for API routes
    if (path.StartsWith("/api/"))
    {
      await _next(context);
      return;
    }

    var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
    if (segments.Length == 0)
    {
      await _next(context);
      return;
    }

    using var scope = _serviceProvider.CreateScope();
    var languageService = scope.ServiceProvider.GetRequiredService<ILanguageService>();

    var potentialLanguageCode = segments[0].ToLower();
    if (potentialLanguageCode.Length == 2 && await languageService.IsLanguageActiveAsync(potentialLanguageCode))
    {
      // Valid language code found, set it in the context
      context.Items["LanguageCode"] = potentialLanguageCode;

      // Remove language code from path for further processing
      context.Request.Path = "/" + string.Join("/", segments.Skip(1));

      await _next(context);
      return;
    }

    // No valid language code found, redirect to default language
    var defaultLanguage = await languageService.GetLanguageAsync("de"); // TODO: Get from configuration
    if (defaultLanguage != null)
    {
      var newPath = $"/{defaultLanguage.IsoCode}{path}";
      context.Response.Redirect(newPath);
      return;
    }

    await _next(context);
  }
}
using API.Features.Languages.Middleware;
using API.Features.Languages.Services;
using Microsoft.Extensions.DependencyInjection;

namespace API.Features.Languages.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddLanguageServices(this IServiceCollection services)
  {
    services.AddScoped<ILanguageService, LanguageService>();
    return services;
  }

  public static IApplicationBuilder UseLanguageMiddleware(this IApplicationBuilder app)
  {
    return app.UseMiddleware<LanguageMiddleware>();
  }
}
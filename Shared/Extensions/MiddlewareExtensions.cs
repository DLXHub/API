using API.Shared.Infrastructure;

namespace API.Shared.Extensions;

public static class MiddlewareExtensions
{
  public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
  {
    return app.UseMiddleware<ExceptionHandlingMiddleware>();
  }
}
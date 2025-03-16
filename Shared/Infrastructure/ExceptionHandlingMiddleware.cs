using System.Net;
using System.Text.Json;
using API.Shared.Models;
using FluentValidation;

namespace API.Shared.Infrastructure;

public class ExceptionHandlingMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<ExceptionHandlingMiddleware> _logger;

  public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      await HandleExceptionAsync(context, ex);
    }
  }

  private async Task HandleExceptionAsync(HttpContext context, Exception exception)
  {
    var response = context.Response;
    response.ContentType = "application/json";

    var (status, errorResponse) = exception switch
    {
      ValidationException validationEx => (
          HttpStatusCode.BadRequest,
          ApiResponse<object>.CreateError(
              "Validation failed",
              validationEx.Errors.Select(e => e.ErrorMessage).ToList()
          )
      ),
      KeyNotFoundException => (
          HttpStatusCode.NotFound,
          ApiResponse<object>.CreateError("The requested resource was not found.")
      ),
      UnauthorizedAccessException => (
          HttpStatusCode.Unauthorized,
          ApiResponse<object>.CreateError("Unauthorized access.")
      ),
      _ => (
          HttpStatusCode.InternalServerError,
          ApiResponse<object>.CreateError("An internal server error occurred.")
      )
    };

    _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

    response.StatusCode = (int)status;
    await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
  }
}
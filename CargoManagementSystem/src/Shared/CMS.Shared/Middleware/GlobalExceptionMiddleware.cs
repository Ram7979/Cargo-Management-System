using System.Text.Json;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CMS.Shared.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
        var (statusCode, response) = exception switch
        {
            NotFoundException ex =>
                (StatusCodes.Status404NotFound,
                 ApiResponse<object>.Fail(ex.Message)),

            ConflictException ex =>
                (StatusCodes.Status409Conflict,
                 ApiResponse<object>.Fail(ex.Message)),

            ValidationException ex =>
                (StatusCodes.Status400BadRequest,
                 ApiResponse<object>.Fail(ex.Errors, ex.Message)),

            UnprocessableException ex =>
                (StatusCodes.Status422UnprocessableEntity,
                 ApiResponse<object>.Fail(ex.Message)),

            ForbiddenException ex =>
                (StatusCodes.Status403Forbidden,
                 ApiResponse<object>.Fail(ex.Message)),

            _ => HandleUnknownException(exception)
        };

        _logger.LogError(exception,
            "Unhandled exception of type {ExceptionType} occurred. StatusCode: {StatusCode}",
            exception.GetType().Name, statusCode);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var json = JsonSerializer.Serialize(response, _jsonOptions);
        await context.Response.WriteAsync(json);
    }

    private static (int statusCode, ApiResponse<object> response) HandleUnknownException(Exception exception)
    {
        return (StatusCodes.Status500InternalServerError,
                ApiResponse<object>.Fail("An unexpected error occurred. Please try again later."));
    }
}

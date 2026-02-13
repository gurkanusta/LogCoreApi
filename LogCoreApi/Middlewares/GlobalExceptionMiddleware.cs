using System.Text.Json;
using FluentValidation;
using LogCoreApi.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LogCoreApi.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error");

            var modelState = new ModelStateDictionary();
            foreach (var error in ex.Errors)
            {
                var key = string.IsNullOrWhiteSpace(error.PropertyName)
                    ? "Validation"
                    : error.PropertyName;

                modelState.AddModelError(key, error.ErrorMessage);
            }

            var details = modelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

            var response = ApiResponse<object>.Fail(
                "ERR-400",
                "Validation failed.",
                context,
                details
            );

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found");

            var response = ApiResponse<object>.Fail(
                "ERR-404",
                ex.Message,
                context
            );

            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            var response = ApiResponse<object>.Fail(
                "ERR-500",
                "An unexpected error occurred.",
                context
            );

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
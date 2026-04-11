// API/Middleware/ExceptionMiddleware.cs
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CoursePlatform.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception: {Message}", ex.Message);
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errors) = ex switch
        {
            NotFoundException e => (404, e.Message, (object?)null),
            ValidationException e => (400, e.Message, e.Errors),
            BadRequestException e => (400, e.Message, null),
            ConflictException e => (409, e.Message, null),
            UnauthorizedException e => (401, e.Message, null),
            ForbiddenException e => (403, e.Message, null),
            _ => (500,
                  _env.IsDevelopment() ? ex.ToString() : "An unexpected error occurred.",
                  null)
        };

        context.Response.StatusCode = statusCode;

        var response = ApiResponse.Fail(statusCode, message, errors);

        var options = new JsonSerializerOptions
        { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
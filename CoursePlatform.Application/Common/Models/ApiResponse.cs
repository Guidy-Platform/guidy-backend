// Application/Common/Models/ApiResponse.cs
namespace CoursePlatform.Application.Common.Models;

public class ApiResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Errors { get; set; }      // validation errors dictionary
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse Success(int statusCode, string message)
        => new() { StatusCode = statusCode, Message = message };

    public static ApiResponse Fail(int statusCode, string message, object? errors = null)
        => new() { StatusCode = statusCode, Message = message, Errors = errors };
}
// Application/Common/Exceptions/UnauthorizedException.cs
namespace CoursePlatform.Application.Common.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Unauthorized access.")
        : base(message) { }
}
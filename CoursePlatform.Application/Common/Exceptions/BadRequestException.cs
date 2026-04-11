// Application/Common/Exceptions/BadRequestException.cs
namespace CoursePlatform.Application.Common.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}
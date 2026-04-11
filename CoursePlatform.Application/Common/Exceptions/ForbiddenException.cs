// Application/Common/Exceptions/ForbiddenException.cs
namespace CoursePlatform.Application.Common.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "You do not have permission to perform this action.")
        : base(message) { }
}
namespace CoursePlatform.Application.Features.Auth.Events;

public class UserRegisteredEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
}
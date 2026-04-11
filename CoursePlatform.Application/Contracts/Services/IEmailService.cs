namespace CoursePlatform.Application.Contracts.Services;

// record in immutable , can't be changed after creation
public record EmailMessage(
    string To,
    string Subject,
    string Body,
    bool IsHtml = true
);

public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken ct = default);
}
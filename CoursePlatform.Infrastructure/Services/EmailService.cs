// Infrastructure/Services/EmailService.cs
using CoursePlatform.Application.Contracts.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace CoursePlatform.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
        => _config = config;

    public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        var host = _config["Email:Host"]!;
        var port = int.Parse(_config["Email:Port"]!);
        var username = _config["Email:Username"]!;
        var password = _config["Email:Password"]!;
        var displayName = _config["Email:DisplayName"] ?? "CoursePlatform";

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network
        };

        var mail = new MailMessage
        {
            From = new MailAddress(username, displayName),
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = message.IsHtml
        };
        mail.To.Add(message.To);

        await client.SendMailAsync(mail, ct);
    }
}
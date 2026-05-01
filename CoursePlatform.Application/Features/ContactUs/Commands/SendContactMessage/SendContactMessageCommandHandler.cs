using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.ContactUs.DTOs;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.ContactUs.Commands.SendContactMessage;

public class SendContactMessageCommandHandler
    : IRequestHandler<SendContactMessageCommand, ContactMessageDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmailService _email;

    public SendContactMessageCommandHandler(
        IUnitOfWork uow,
        IEmailService email)
    {
        _uow = uow;
        _email = email;
    }

    public async Task<ContactMessageDto> Handle(
        SendContactMessageCommand request, CancellationToken ct)
    {
        var message = new ContactMessage
        {
            FullName = request.FullName,
            Email = request.Email,
            Subject = request.Subject,
            Message = request.Message,
            IpAddress = request.IpAddress   // ← بييجي من الـ Controller
        };

        await _uow.Repository<ContactMessage>().AddAsync(message, ct);
        await _uow.CompleteAsync(ct);

        // Email للـ Admin
        await _email.SendAsync(new EmailMessage(
            To: "support@guidy.com",
            Subject: $"[Contact Us] {request.Subject}",
            Body: $"""
                <h3>New Contact Message</h3>
                <p><strong>From:</strong> {request.FullName} ({request.Email})</p>
                <p><strong>Subject:</strong> {request.Subject}</p>
                <p><strong>Message:</strong></p>
                <p>{request.Message}</p>
                """
        ));

        // Confirmation للـ User
        await _email.SendAsync(new EmailMessage(
            To: request.Email,
            Subject: "We received your message — Guidy Platform",
            Body: $"""
                <h3>Hi {request.FullName},</h3>
                <p>Thank you for contacting us!</p>
                <p>We received your message and will get back to you within 24 hours.</p>
                <br/>
                <p><strong>Your message:</strong></p>
                <p>{request.Message}</p>
                <br/>
                <p>Best regards,<br/>Guidy Platform Team</p>
                """
        ));

        return MapToDto(message);
    }

    internal static ContactMessageDto MapToDto(ContactMessage m) => new()
    {
        Id = m.Id,
        FullName = m.FullName,
        Email = m.Email,
        Subject = m.Subject,
        Message = m.Message,
        Status = m.Status.ToString(),
        AdminReply = m.AdminReply,
        RepliedAt = m.RepliedAt,
        CreatedAt = m.CreatedAt
    };
}
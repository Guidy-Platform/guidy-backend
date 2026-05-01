// Application/Features/ContactUs/Commands/ReplyToContactMessage/ReplyToContactMessageCommandHandler.cs
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.ContactUs.Commands.SendContactMessage;
using CoursePlatform.Application.Features.ContactUs.DTOs;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.ContactUs.Commands.ReplyToContactMessage;

public class ReplyToContactMessageCommandHandler
    : IRequestHandler<ReplyToContactMessageCommand, ContactMessageDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmailService _email;

    public ReplyToContactMessageCommandHandler(
        IUnitOfWork uow, IEmailService email)
    {
        _uow = uow;
        _email = email;
    }

    public async Task<ContactMessageDto> Handle(
        ReplyToContactMessageCommand request, CancellationToken ct)
    {
        var message = await _uow.Repository<ContactMessage>()
                                .GetByIdAsync(request.MessageId, ct)
            ?? throw new NotFoundException("ContactMessage", request.MessageId);

        // Update الـ message
        message.AdminReply = request.Reply;
        message.Status = ContactMessageStatus.Replied;
        message.RepliedAt = DateTime.UtcNow;

        _uow.Repository<ContactMessage>().Update(message);
        await _uow.CompleteAsync(ct);

        await _email.SendAsync(new EmailMessage(
            To: message.Email,
            Subject: $"Re: {message.Subject} — Guidy Platform",
            Body: $"""
                <h3>Hi {message.FullName},</h3>
                <p>Thank you for contacting Guidy Platform.</p>
                <p>Here is our response to your message:</p>
                <hr/>
                <p>{request.Reply}</p>
                <hr/>
                <p><strong>Your original message:</strong></p>
                <p>{message.Message}</p>
                <br/>
                <p>Best regards,<br/>Guidy Platform Support Team</p>
                """
        ));

        return SendContactMessageCommandHandler.MapToDto(message);
    }
}
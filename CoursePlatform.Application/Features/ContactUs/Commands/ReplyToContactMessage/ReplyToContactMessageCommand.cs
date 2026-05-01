using CoursePlatform.Application.Features.ContactUs.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.ContactUs.Commands.ReplyToContactMessage;

public record ReplyToContactMessageCommand(
    int MessageId,
    string Reply
) : IRequest<ContactMessageDto>;
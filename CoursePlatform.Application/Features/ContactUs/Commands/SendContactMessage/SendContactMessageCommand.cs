using CoursePlatform.Application.Features.ContactUs.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.ContactUs.Commands.SendContactMessage;

public record SendContactMessageCommand(
    string FullName,
    string Email,
    string Subject,
    string Message,
    string? IpAddress = null
) : IRequest<ContactMessageDto>;
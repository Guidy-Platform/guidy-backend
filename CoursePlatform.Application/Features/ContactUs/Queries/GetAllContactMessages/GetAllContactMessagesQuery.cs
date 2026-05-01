using CoursePlatform.Application.Features.ContactUs.DTOs;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.ContactUs.Queries.GetAllContactMessages;

public record GetAllContactMessagesQuery(
    ContactMessageStatus? Status = null
) : IRequest<IReadOnlyList<ContactMessageDto>>;
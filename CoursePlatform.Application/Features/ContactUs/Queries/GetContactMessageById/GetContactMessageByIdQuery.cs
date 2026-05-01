using CoursePlatform.Application.Features.ContactUs.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.ContactUs.Queries.GetContactMessageById;

public record GetContactMessageByIdQuery(int Id) : IRequest<ContactMessageDto>;
// Application/Features/ContactUs/Queries/GetContactMessageById/GetContactMessageByIdQueryHandler.cs
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Features.ContactUs.Commands.SendContactMessage;
using CoursePlatform.Application.Features.ContactUs.DTOs;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.ContactUs.Queries.GetContactMessageById;

public class GetContactMessageByIdQueryHandler
    : IRequestHandler<GetContactMessageByIdQuery, ContactMessageDto>
{
    private readonly IUnitOfWork _uow;

    public GetContactMessageByIdQueryHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task<ContactMessageDto> Handle(
        GetContactMessageByIdQuery request, CancellationToken ct)
    {
        var message = await _uow.Repository<ContactMessage>()
                                .GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("ContactMessage", request.Id);

        // Mark as Read لو New
        if (message.Status == Domain.Enums.ContactMessageStatus.New)
        {
            message.Status = Domain.Enums.ContactMessageStatus.Read;
            _uow.Repository<ContactMessage>().Update(message);
            await _uow.CompleteAsync(ct);
        }

        return SendContactMessageCommandHandler.MapToDto(message);
    }
}
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Features.ContactUs.Commands.SendContactMessage;
using CoursePlatform.Application.Features.ContactUs.DTOs;
using CoursePlatform.Application.Features.ContactUs.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.ContactUs.Queries.GetAllContactMessages;

public class GetAllContactMessagesQueryHandler
    : IRequestHandler<GetAllContactMessagesQuery, IReadOnlyList<ContactMessageDto>>
{
    private readonly IUnitOfWork _uow;

    public GetAllContactMessagesQueryHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task<IReadOnlyList<ContactMessageDto>> Handle(
        GetAllContactMessagesQuery request, CancellationToken ct)
    {
        var spec = new AllContactMessagesSpec(request.Status);
        var messages = await _uow.Repository<ContactMessage>()
                                 .GetAllWithSpecAsync(spec, ct);

        return messages
            .Select(SendContactMessageCommandHandler.MapToDto)
            .ToList();
    }
}
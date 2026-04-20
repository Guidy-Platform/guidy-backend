using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Features.Payouts.Commands.RequestPayout;
using CoursePlatform.Application.Features.Payouts.DTOs;
using CoursePlatform.Application.Features.Payouts.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Payouts.Queries.GetAllPayouts;

public class GetAllPayoutsQueryHandler
    : IRequestHandler<GetAllPayoutsQuery, IReadOnlyList<PayoutDto>>
{
    private readonly IUnitOfWork _uow;

    public GetAllPayoutsQueryHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task<IReadOnlyList<PayoutDto>> Handle(
        GetAllPayoutsQuery request, CancellationToken ct)
    {
        var spec = new AllPayoutsSpec(request.Status);
        var payouts = await _uow.Repository<Payout>()
                                .GetAllWithSpecAsync(spec, ct);

        return payouts
            .Select(RequestPayoutCommandHandler.MapToDto)
            .ToList();
    }
}
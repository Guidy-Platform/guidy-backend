using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Payouts.Commands.RequestPayout;
using CoursePlatform.Application.Features.Payouts.DTOs;
using CoursePlatform.Application.Features.Payouts.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Payouts.Queries.GetMyPayouts;

public class GetMyPayoutsQueryHandler
    : IRequestHandler<GetMyPayoutsQuery, IReadOnlyList<PayoutDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetMyPayoutsQueryHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<PayoutDto>> Handle(
        GetMyPayoutsQuery request, CancellationToken ct)
    {
        var instructorId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var spec = new PayoutsByInstructorSpec(instructorId);
        var payouts = await _uow.Repository<Payout>()
                                .GetAllWithSpecAsync(spec, ct);

        return payouts
            .Select(RequestPayoutCommandHandler.MapToDto)
            .ToList();
    }
}
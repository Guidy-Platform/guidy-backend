using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Subscriptions.Commands.Subscribe;
using CoursePlatform.Application.Features.Subscriptions.DTOs;
using CoursePlatform.Application.Features.Subscriptions.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Subscriptions.Queries.GetMySubscription;

public class GetMySubscriptionQueryHandler
    : IRequestHandler<GetMySubscriptionQuery, UserSubscriptionDto?>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetMySubscriptionQueryHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<UserSubscriptionDto?> Handle(
        GetMySubscriptionQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var spec = new SubscriptionByUserSpec(userId);
        var subscription = await _uow.Repository<UserSubscription>()
                                     .GetEntityWithSpecAsync(spec, ct);

        if (subscription is null) return null;

        return SubscribeCommandHandler.MapToDto(subscription, subscription.Plan);
    }
}
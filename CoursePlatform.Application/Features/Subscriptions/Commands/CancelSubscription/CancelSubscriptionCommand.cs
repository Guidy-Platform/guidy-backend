using CoursePlatform.Application.Features.Subscriptions.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Subscriptions.Commands.CancelSubscription;

public record CancelSubscriptionCommand : IRequest<UserSubscriptionDto>;
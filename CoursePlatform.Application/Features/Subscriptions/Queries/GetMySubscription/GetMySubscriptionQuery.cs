using CoursePlatform.Application.Features.Subscriptions.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Subscriptions.Queries.GetMySubscription;

public record GetMySubscriptionQuery : IRequest<UserSubscriptionDto?>;
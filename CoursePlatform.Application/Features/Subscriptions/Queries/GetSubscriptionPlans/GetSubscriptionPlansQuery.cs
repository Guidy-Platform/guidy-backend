using CoursePlatform.Application.Features.Subscriptions.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Subscriptions.Queries.GetSubscriptionPlans;

public record GetSubscriptionPlansQuery : IRequest<IReadOnlyList<SubscriptionPlanDto>>;
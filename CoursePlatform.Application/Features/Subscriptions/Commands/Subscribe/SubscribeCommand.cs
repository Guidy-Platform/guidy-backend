using CoursePlatform.Application.Features.Subscriptions.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Subscriptions.Commands.Subscribe;

public record SubscribeCommand(int PlanId) : IRequest<UserSubscriptionDto>;
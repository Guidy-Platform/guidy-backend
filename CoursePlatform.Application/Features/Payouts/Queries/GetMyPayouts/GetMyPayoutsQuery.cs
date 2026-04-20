using CoursePlatform.Application.Features.Payouts.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Payouts.Queries.GetMyPayouts;

public record GetMyPayoutsQuery : IRequest<IReadOnlyList<PayoutDto>>;
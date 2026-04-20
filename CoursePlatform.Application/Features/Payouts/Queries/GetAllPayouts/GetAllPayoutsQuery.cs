using CoursePlatform.Application.Features.Payouts.DTOs;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Payouts.Queries.GetAllPayouts;

public record GetAllPayoutsQuery(
    PayoutStatus? Status = null
) : IRequest<IReadOnlyList<PayoutDto>>;
using CoursePlatform.Application.Features.Payouts.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Payouts.Commands.ApprovePayout;

public record ApprovePayoutCommand(
    int PayoutId,
    string? Notes = null
) : IRequest<PayoutDto>;
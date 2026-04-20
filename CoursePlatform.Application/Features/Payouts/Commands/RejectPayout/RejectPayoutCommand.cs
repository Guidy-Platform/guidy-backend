using CoursePlatform.Application.Features.Payouts.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Payouts.Commands.RejectPayout;

public record RejectPayoutCommand(
    int PayoutId,
    string Reason
) : IRequest<PayoutDto>;
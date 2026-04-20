using CoursePlatform.Application.Features.Payouts.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Payouts.Commands.RequestPayout;

public record RequestPayoutCommand(decimal Amount) : IRequest<PayoutDto>;
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Payouts.Commands.RequestPayout;
using CoursePlatform.Application.Features.Payouts.DTOs;
using CoursePlatform.Application.Features.Payouts.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Payouts.Commands.RejectPayout;

public class RejectPayoutCommandHandler
    : IRequestHandler<RejectPayoutCommand, PayoutDto>
{
    private readonly IUnitOfWork _uow;
    private readonly INotificationService _notifications;

    public RejectPayoutCommandHandler(
        IUnitOfWork uow,
        INotificationService notifications)
    {
        _uow = uow;
        _notifications = notifications;
    }

    public async Task<PayoutDto> Handle(
        RejectPayoutCommand request, CancellationToken ct)
    {
        var spec = new PayoutByIdSpec(request.PayoutId);
        var payout = await _uow.Repository<Payout>()
                               .GetEntityWithSpecAsync(spec, ct)
            ?? throw new NotFoundException("Payout", request.PayoutId);

        if (payout.Status != PayoutStatus.Pending)
            throw new BadRequestException(
                "Only pending payouts can be rejected.");

        //  Payout update
        payout.Status = PayoutStatus.Rejected;
        payout.RejectionReason = request.Reason;
        payout.ProcessedAt = DateTime.UtcNow;
        _uow.Repository<Payout>().Update(payout);

        // return the amount to the available balance
        var walletSpec = new WalletByInstructorSpec(payout.InstructorId);
        var wallet = await _uow.Repository<InstructorWallet>()
                                   .GetEntityWithSpecAsync(walletSpec, ct);

        if (wallet is not null)
        {
            wallet.PendingAmount = Math.Max(
                0, wallet.PendingAmount - payout.Amount);
            wallet.AvailableBalance += payout.Amount;
            _uow.Repository<InstructorWallet>().Update(wallet);
        }

        await _uow.CompleteAsync(ct);

        // Notification for Instructor
        await _notifications.SendAsync(
            userId: payout.InstructorId,
            title: "Payout Request Rejected",
            message: $"Your payout of ${payout.Amount:F2} was rejected: {request.Reason}",
            type: NotificationType.SystemMessage,
            actionUrl: "/instructor/payouts",
            ct: ct);

        return RequestPayoutCommandHandler.MapToDto(payout);
    }
}
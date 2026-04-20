using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Payouts.Commands.RequestPayout;
using CoursePlatform.Application.Features.Payouts.DTOs;
using CoursePlatform.Application.Features.Payouts.Helpers;
using CoursePlatform.Application.Features.Payouts.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Payouts.Commands.ApprovePayout;

public class ApprovePayoutCommandHandler
    : IRequestHandler<ApprovePayoutCommand, PayoutDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IStripeConnectService _stripe;
    private readonly INotificationService _notifications;

    public ApprovePayoutCommandHandler(
        IUnitOfWork uow,
        IStripeConnectService stripe,
        INotificationService notifications)
    {
        _uow = uow;
        _stripe = stripe;
        _notifications = notifications;
    }

    public async Task<PayoutDto> Handle(
        ApprovePayoutCommand request, CancellationToken ct)
    {
        // 1. جيب الـ Payout
        var spec = new PayoutByIdSpec(request.PayoutId);
        var payout = await _uow.Repository<Payout>()
                               .GetEntityWithSpecAsync(spec, ct)
            ?? throw new NotFoundException("Payout", request.PayoutId);

        if (payout.Status != PayoutStatus.Pending)
            throw new BadRequestException(
                "Only pending payouts can be approved.");

        // 2. جيب الـ wallet للـ Stripe Account ID
        var walletSpec = new WalletByInstructorSpec(payout.InstructorId);
        var wallet = await _uow.Repository<InstructorWallet>()
                                   .GetEntityWithSpecAsync(walletSpec, ct)
            ?? throw new BadRequestException(
                "Instructor wallet not found.");

        if (!wallet.IsStripeConnected ||
            string.IsNullOrEmpty(wallet.StripeAccountId))
            throw new BadRequestException(
                "Instructor has not connected their Stripe account.");

        // 3. عمل Stripe Transfer
        var transfer = await _stripe.TransferAsync(
            wallet.StripeAccountId,
            payout.Amount,
            "usd",
            payout.Id,
            ct);

        if (!transfer.IsSuccess)
        {
            payout.Status = PayoutStatus.Failed;
            payout.Notes = $"Stripe Error: {transfer.ErrorMessage}";
            _uow.Repository<Payout>().Update(payout);
            await _uow.CompleteAsync(ct);

            throw new BadRequestException(
                $"Stripe transfer failed: {transfer.ErrorMessage}");
        }

        // 4. حدث الـ Payout
        payout.Status = PayoutStatus.Approved;
        payout.StripeTransferId = transfer.TransferId;
        payout.ProcessedAt = DateTime.UtcNow;
        payout.Notes = request.Notes;
        _uow.Repository<Payout>().Update(payout);

        // 5. حدث الـ Wallet
        wallet.TotalPaidOut += payout.Amount;
        wallet.PendingAmount = Math.Max(
            0, wallet.PendingAmount - payout.Amount);
        _uow.Repository<InstructorWallet>().Update(wallet);

        await _uow.CompleteAsync(ct);

        // 6. Notification للـ Instructor
        await _notifications.SendAsync(
            userId: payout.InstructorId,
            title: "Payout Approved!",
            message: $"Your payout of ${payout.Amount:F2} has been processed.",
            type: Domain.Enums.NotificationType.SystemMessage,
            actionUrl: "/instructor/payouts",
            ct: ct);

        return RequestPayoutCommandHandler.MapToDto(payout);
    }
}
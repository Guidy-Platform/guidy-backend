using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Payouts.DTOs;
using CoursePlatform.Application.Features.Payouts.Helpers;
using CoursePlatform.Application.Features.Payouts.Specifications;
using CoursePlatform.Domain.Constants;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Payouts.Commands.RequestPayout;

public class RequestPayoutCommandHandler
    : IRequestHandler<RequestPayoutCommand, PayoutDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public RequestPayoutCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<PayoutDto> Handle(
        RequestPayoutCommand request, CancellationToken ct)
    {
        var instructorId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        // 1. جيب الـ wallet وحدث الـ balance
        await WalletHelper.RecalculateBalanceAsync(instructorId, _uow, ct);
        var wallet = await WalletHelper.GetOrCreateWalletAsync(
            instructorId, _uow, ct);

        // 2. تحقق إن Stripe Connected
        if (!wallet.IsStripeConnected ||
            string.IsNullOrEmpty(wallet.StripeAccountId))
            throw new BadRequestException(
                "You must connect your Stripe account before requesting a payout. " +
                "Use POST /api/payouts/connect-stripe.");

        // 3. تحقق من الـ available balance
        if (request.Amount > wallet.AvailableBalance)
            throw new BadRequestException(
                $"Insufficient balance. Available: ${wallet.AvailableBalance:F2}, " +
                $"Requested: ${request.Amount:F2}");

        // 4. تحقق مفيش Pending payout تاني
        var pendingSpec = new PendingPayoutsByInstructorSpec(instructorId);
        var hasPending = await _uow.Repository<Payout>()
                                       .AnyAsync(pendingSpec, ct);
        if (hasPending)
            throw new BadRequestException(
                "You already have a pending payout request. " +
                "Please wait for it to be processed.");

        // 5. حسب الـ Platform Fee
        var platformFee = Math.Round(
            request.Amount * PlatformConstants.PlatformCommissionRate, 2);

        // 6. إنشاء الـ Payout
        var payout = new Payout
        {
            InstructorId = instructorId,
            Amount = request.Amount,
            PlatformFee = platformFee,
            Status = PayoutStatus.Pending
        };

        await _uow.Repository<Payout>().AddAsync(payout, ct);

        // 7. حدث الـ wallet PendingAmount
        wallet.PendingAmount += request.Amount;
        wallet.AvailableBalance = Math.Max(
            0, wallet.AvailableBalance - request.Amount);
        _uow.Repository<InstructorWallet>().Update(wallet);

        await _uow.CompleteAsync(ct);

        return MapToDto(payout);
    }

    internal static PayoutDto MapToDto(Payout p) => new()
    {
        Id = p.Id,
        InstructorId = p.InstructorId,
        InstructorName = p.Instructor?.FullName ?? string.Empty,
        Amount = p.Amount,
        PlatformFee = p.PlatformFee,
        Status = p.Status.ToString(),
        RejectionReason = p.RejectionReason,
        StripeTransferId = p.StripeTransferId,
        Notes = p.Notes,
        CreatedAt = p.CreatedAt,
        ProcessedAt = p.ProcessedAt
    };
}
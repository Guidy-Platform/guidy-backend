using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Payouts.DTOs;
using CoursePlatform.Application.Features.Payouts.Helpers;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Payouts.Queries.GetMyWallet;

public class GetMyWalletQueryHandler
    : IRequestHandler<GetMyWalletQuery, WalletDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetMyWalletQueryHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<WalletDto> Handle(
        GetMyWalletQuery request, CancellationToken ct)
    {
        var instructorId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        // Recalculate the balance first
        await WalletHelper.RecalculateBalanceAsync(instructorId, _uow, ct);

        var wallet = await WalletHelper.GetOrCreateWalletAsync(
            instructorId, _uow, ct);

        return new WalletDto
        {
            TotalEarned = wallet.TotalEarned,
            TotalPaidOut = wallet.TotalPaidOut,
            PendingAmount = wallet.PendingAmount,
            AvailableBalance = wallet.AvailableBalance,
            IsStripeConnected = wallet.IsStripeConnected,
            StripeAccountId = wallet.StripeAccountId
        };
    }
}
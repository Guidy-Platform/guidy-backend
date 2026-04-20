using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Features.Payouts.Specifications;
using CoursePlatform.Domain.Constants;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Features.Payouts.Helpers;

public static class WalletHelper
{
    /// <summary>
    /// يجيب أو ينشئ الـ wallet للـ Instructor
    /// </summary>
    public static async Task<InstructorWallet> GetOrCreateWalletAsync(
        Guid instructorId,
        IUnitOfWork uow,
        CancellationToken ct)
    {
        var spec = new WalletByInstructorSpec(instructorId);
        var wallet = await uow.Repository<InstructorWallet>()
                              .GetEntityWithSpecAsync(spec, ct);

        if (wallet is not null) return wallet;

        wallet = new InstructorWallet
        {
            InstructorId = instructorId
        };

        await uow.Repository<InstructorWallet>().AddAsync(wallet, ct);
        await uow.CompleteAsync(ct);

        return wallet;
    }

    /// <summary>
    /// يحسب ويحدث الـ wallet balance من الـ Revenue الفعلي
    /// </summary>
    public static async Task RecalculateBalanceAsync(
        Guid instructorId,
        IUnitOfWork uow,
        CancellationToken ct)
    {
        var wallet = await GetOrCreateWalletAsync(instructorId, uow, ct);

        // إجمالي الـ Revenue من الـ completed orders (70% للـ Instructor)
        var orderItemsSpec = new CompletedOrderItemsByInstructorSpec(instructorId);
        var orderItems = await uow.Repository<OrderItem>()
                                      .GetAllWithSpecAsync(orderItemsSpec, ct);

        var totalRevenue = orderItems.Sum(i => i.Price);
        var instructorShare = Math.Round(
            totalRevenue * PlatformConstants.InstructorShareRate, 2);

        // إجمالي الـ completed payouts
        var paidOutSpec = new CompletedPayoutsByInstructorSpec(instructorId);
        var paidOut = await uow.Repository<Payout>()
                                   .GetAllWithSpecAsync(paidOutSpec, ct);
        var totalPaidOut = paidOut.Sum(p => p.Amount);

        // الـ pending payouts
        var pendingSpec = new PendingPayoutsByInstructorSpec(instructorId);
        var pending = await uow.Repository<Payout>()
                                   .GetAllWithSpecAsync(pendingSpec, ct);
        var pendingAmount = pending.Sum(p => p.Amount);

        wallet.TotalEarned = instructorShare;
        wallet.TotalPaidOut = totalPaidOut;
        wallet.PendingAmount = pendingAmount;
        wallet.AvailableBalance = Math.Max(
            0, instructorShare - totalPaidOut - pendingAmount);

        uow.Repository<InstructorWallet>().Update(wallet);
        await uow.CompleteAsync(ct);
    }
}
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Features.Subscriptions.DTOs;
using CoursePlatform.Application.Features.Subscriptions.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Subscriptions.Queries.GetSubscriptionPlans;

public class GetSubscriptionPlansQueryHandler
    : IRequestHandler<GetSubscriptionPlansQuery, IReadOnlyList<SubscriptionPlanDto>>
{
    private readonly IUnitOfWork _uow;

    public GetSubscriptionPlansQueryHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task<IReadOnlyList<SubscriptionPlanDto>> Handle(
        GetSubscriptionPlansQuery request, CancellationToken ct)
    {
        var spec = new AllSubscriptionPlansSpec(activeOnly: true);
        var plans = await _uow.Repository<SubscriptionPlan>()
                              .GetAllWithSpecAsync(spec, ct);

        // جيب الـ monthly plan للـ comparison
        var monthlyPlan = plans.FirstOrDefault(
            p => p.BillingInterval == BillingInterval.Monthly);
        var monthlyPrice = monthlyPlan?.Price ?? 29.99m;

        return plans.Select(p =>
        {
            var pricePerMonth = p.BillingInterval == BillingInterval.Annual
                ? Math.Round(p.Price / 12, 2)
                : p.Price;

            var savePercent = p.BillingInterval == BillingInterval.Annual
                ? (int)Math.Round(
                    (1 - pricePerMonth / monthlyPrice) * 100)
                : (int?)null;

            return new SubscriptionPlanDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                BillingInterval = p.BillingInterval.ToString(),
                PricePerMonth = pricePerMonth,
                SavePercent = savePercent,
                IsActive = p.IsActive
            };
        }).ToList();
    }
}
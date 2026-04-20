using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.InstructorDashboard.DTOs;
using CoursePlatform.Application.Features.InstructorDashboard.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.InstructorDashboard.Queries.GetRevenueStats;

public class GetRevenueStatsQueryHandler
    : IRequestHandler<GetRevenueStatsQuery, RevenueStatsDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetRevenueStatsQueryHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<RevenueStatsDto> Handle(
        GetRevenueStatsQuery request, CancellationToken ct)
    {
        var instructorId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var months = Math.Clamp(request.Months, 1, 24);
        var startOf = new DateTime(
            DateTime.UtcNow.AddMonths(-months + 1).Year,
            DateTime.UtcNow.AddMonths(-months + 1).Month,
            1, 0, 0, 0, DateTimeKind.Utc);

        var spec = new CompletedOrderItemsByInstructorAndPeriodSpec(
            instructorId, startOf);
        var orderItems = await _uow.Repository<OrderItem>()
                                   .GetAllWithSpecAsync(spec, ct);

        // Group by year/month في الـ memory
        var grouped = orderItems
            .GroupBy(i => new
            {
                Year = i.Order.PaidAt!.Value.Year,
                Month = i.Order.PaidAt!.Value.Month
            })
            .ToDictionary(g => g.Key, g => new
            {
                Amount = g.Sum(x => x.Price),
                Count = g.Count()
            });

        // ابني قايمة بكل الشهور (حتى اللي فيها صفر)
        var monthlyRevenue = new List<MonthlyStatDto>();
        for (var i = 0; i < months; i++)
        {
            var date = startOf.AddMonths(i);
            var key = new { Year = date.Year, Month = date.Month };

            grouped.TryGetValue(key, out var data);

            monthlyRevenue.Add(new MonthlyStatDto
            {
                Year = date.Year,
                Month = date.Month,
                Label = date.ToString("MMM yyyy"),
                Amount = data?.Amount ?? 0,
                Count = data?.Count ?? 0
            });
        }

        var totalRevenue = monthlyRevenue.Sum(m => m.Amount);
        var best = monthlyRevenue.MaxBy(m => m.Amount);

        return new RevenueStatsDto
        {
            MonthlyRevenue = monthlyRevenue,
            TotalRevenue = totalRevenue,
            AveragePerMonth = months > 0
                ? Math.Round(totalRevenue / months, 2) : 0,
            BestMonthRevenue = best?.Amount ?? 0,
            BestMonth = best?.Label ?? string.Empty
        };
    }
}
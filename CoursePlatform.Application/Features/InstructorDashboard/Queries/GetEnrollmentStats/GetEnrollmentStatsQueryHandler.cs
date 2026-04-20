using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.InstructorDashboard.DTOs;
using CoursePlatform.Application.Features.InstructorDashboard.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.InstructorDashboard.Queries.GetEnrollmentStats;

public class GetEnrollmentStatsQueryHandler
    : IRequestHandler<GetEnrollmentStatsQuery, EnrollmentStatsDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetEnrollmentStatsQueryHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<EnrollmentStatsDto> Handle(
        GetEnrollmentStatsQuery request, CancellationToken ct)
    {
        var instructorId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var months = Math.Clamp(request.Months, 1, 24);
        var now = DateTime.UtcNow;

        var startOf = new DateTime(
            now.AddMonths(-months + 1).Year,
            now.AddMonths(-months + 1).Month,
            1, 0, 0, 0, DateTimeKind.Utc);

        var startOfThisMonth = new DateTime(
            now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var startOfLastMonth = startOfThisMonth.AddMonths(-1);

        // جيب الـ enrollments في الـ period
        var periodSpec = new EnrollmentsByInstructorAndPeriodSpec(
            instructorId, startOf);
        var periodEnroll = await _uow.Repository<Enrollment>()
                                     .GetAllWithSpecAsync(periodSpec, ct);

        // Group by year/month
        var grouped = periodEnroll
            .GroupBy(e => new
            {
                Year = e.EnrolledAt.Year,
                Month = e.EnrolledAt.Month
            })
            .ToDictionary(g => g.Key, g => g.Count());

        var monthly = new List<MonthlyStatDto>();
        for (var i = 0; i < months; i++)
        {
            var date = startOf.AddMonths(i);
            var key = new { Year = date.Year, Month = date.Month };

            grouped.TryGetValue(key, out var count);

            monthly.Add(new MonthlyStatDto
            {
                Year = date.Year,
                Month = date.Month,
                Label = date.ToString("MMM yyyy"),
                Amount = 0,
                Count = count
            });
        }

        var thisMonth = periodEnroll
            .Count(e => e.EnrolledAt >= startOfThisMonth);
        var lastMonth = periodEnroll
            .Count(e => e.EnrolledAt >= startOfLastMonth &&
                        e.EnrolledAt < startOfThisMonth);

        var growth = lastMonth > 0
            ? Math.Round(
                (double)(thisMonth - lastMonth) / lastMonth * 100, 1)
            : thisMonth > 0 ? 100.0 : 0.0;

        // إجمالي كل الـ enrollments
        var totalSpec = new EnrollmentsByInstructorSpec(instructorId);
        var totalCount = await _uow.Repository<Enrollment>()
                                   .CountAsync(totalSpec, ct);

        return new EnrollmentStatsDto
        {
            MonthlyEnrollments = monthly,
            TotalEnrollments = totalCount,
            ThisMonthCount = thisMonth,
            LastMonthCount = lastMonth,
            GrowthPercent = growth
        };
    }
}
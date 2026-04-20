// Application/Features/InstructorDashboard/Queries/GetDashboardSummary/GetDashboardSummaryQueryHandler.cs
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.InstructorDashboard.DTOs;
using CoursePlatform.Application.Features.InstructorDashboard.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.InstructorDashboard.Queries.GetDashboardSummary;

public class GetDashboardSummaryQueryHandler
    : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetDashboardSummaryQueryHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<DashboardSummaryDto> Handle(
        GetDashboardSummaryQuery request, CancellationToken ct)
    {
        var instructorId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(
            now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var startOfLast = startOfMonth.AddMonths(-1);

        // ─── Courses ──────────────────────────────────────────────
        var coursesSpec = new CoursesByInstructorSpec(instructorId);
        var courses = await _uow.Repository<Course>()
                                    .GetAllWithSpecAsync(coursesSpec, ct);

        // ─── Enrollments ──────────────────────────────────────────
        var enrollSpec = new EnrollmentsByInstructorSpec(instructorId);
        var enrollments = await _uow.Repository<Enrollment>()
                                    .GetAllWithSpecAsync(enrollSpec, ct);

        var thisMonthEnrollments = enrollments
            .Count(e => e.EnrolledAt >= startOfMonth);

        // ─── Revenue ──────────────────────────────────────────────
 

        var orderItemsSpec = new CompletedOrderItemsByInstructorSpec(instructorId);
        var orderItems = await _uow.Repository<OrderItem>()
                                       .GetAllWithSpecAsync(orderItemsSpec, ct);

        var totalRevenue = orderItems.Sum(i => i.Price);
        var revenueThisMonth = orderItems
            .Where(i => i.Order.PaidAt >= startOfMonth)
            .Sum(i => i.Price);
        var revenueLastMonth = orderItems
            .Where(i => i.Order.PaidAt >= startOfLast &&
                        i.Order.PaidAt < startOfMonth)
            .Sum(i => i.Price);

        var revenueGrowth = revenueLastMonth > 0
            ? Math.Round(
                (double)(revenueThisMonth - revenueLastMonth)
                / (double)revenueLastMonth * 100, 1)
            : revenueThisMonth > 0 ? 100.0 : 0.0;

        // ─── Ratings ──────────────────────────────────────────────
        var reviewsSpec = new ReviewsByInstructorSpec(instructorId);
        var reviews = await _uow.Repository<Review>()
                                    .GetAllWithSpecAsync(reviewsSpec, ct);

        var avgRating = reviews.Any()
            ? Math.Round(reviews.Average(r => r.Rating), 1)
            : 0;

        return new DashboardSummaryDto
        {
            TotalCourses = courses.Count,
            PublishedCourses = courses.Count(
                c => c.Status == CourseStatus.Published),
            DraftCourses = courses.Count(
                c => c.Status == CourseStatus.Draft),
            PendingCourses = courses.Count(
                c => c.Status == CourseStatus.UnderReview),
            TotalStudents = enrollments
                .Select(e => e.StudentId).Distinct().Count(),
            NewStudentsThisMonth = thisMonthEnrollments,
            TotalRevenue = totalRevenue,
            RevenueThisMonth = revenueThisMonth,
            RevenueLastMonth = revenueLastMonth,
            RevenueGrowthPercent = revenueGrowth,
            AverageRating = avgRating,
            TotalReviews = reviews.Count
        };
    }
}
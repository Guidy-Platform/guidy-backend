// Application/Features/InstructorDashboard/Queries/GetTopCourses/GetTopCoursesQueryHandler.cs
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.InstructorDashboard.DTOs;
using CoursePlatform.Application.Features.InstructorDashboard.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.InstructorDashboard.Queries.GetTopCourses;

public class GetTopCoursesQueryHandler
    : IRequestHandler<GetTopCoursesQuery, IReadOnlyList<TopCourseDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetTopCoursesQueryHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<TopCourseDto>> Handle(
        GetTopCoursesQuery request, CancellationToken ct)
    {
        var instructorId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var top = Math.Clamp(request.Top, 1, 20);

        // 1. جيب الـ courses مع الـ Enrollments
        var coursesSpec = new CourseWithStatsSpec(instructorId);
        var courses = await _uow.Repository<Course>()
                                    .GetAllWithSpecAsync(coursesSpec, ct);

        // 2. جيب الـ completed order items للـ revenue
        var orderItemsSpec = new CompletedOrderItemsByInstructorSpec(instructorId);
        var orderItems = await _uow.Repository<OrderItem>()
                                       .GetAllWithSpecAsync(orderItemsSpec, ct);

        // 3. Group الـ revenue بالـ CourseId
        var revenuePerCourse = orderItems
            .GroupBy(i => i.CourseId)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.Price));

        // 4. ابني الـ DTOs
        var dtos = courses.Select(c => new TopCourseDto
        {
            CourseId = c.Id,
            Title = c.Title,
            ThumbnailUrl = c.ThumbnailUrl,
            Status = c.Status.ToString(),
            Enrollments = c.Enrollments.Count,
            Revenue = revenuePerCourse.TryGetValue(c.Id, out var rev)
                           ? rev : 0,
            Rating = c.AverageRating,
            Reviews = c.TotalRatings
        }).ToList();

        // 5. Sort
        var sorted = request.SortBy.ToLower() switch
        {
            "revenue" => dtos.OrderByDescending(c => c.Revenue),
            "rating" => dtos.OrderByDescending(c => c.Rating),
            _ => dtos.OrderByDescending(c => c.Enrollments)
        };

        return sorted.Take(top).ToList();
    }
}
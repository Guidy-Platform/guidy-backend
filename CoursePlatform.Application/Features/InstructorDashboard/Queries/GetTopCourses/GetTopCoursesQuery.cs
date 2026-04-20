using CoursePlatform.Application.Features.InstructorDashboard.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.InstructorDashboard.Queries.GetTopCourses;

public record GetTopCoursesQuery(
    int Top = 5,
    string SortBy = "enrollments"  // "enrollments" | "revenue" | "rating"
) : IRequest<IReadOnlyList<TopCourseDto>>;
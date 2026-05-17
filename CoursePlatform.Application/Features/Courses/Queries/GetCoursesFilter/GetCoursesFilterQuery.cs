using CoursePlatform.Application.Features.Courses.Queries.GetMyCourses;
using MediatR;

namespace CoursePlatform.Application.Features.Courses.Queries.GetCoursesFilter;

public record GetCoursesFilterQuery(string? Search = null)
    : IRequest<IReadOnlyList<CourseFilterItemDto>>;
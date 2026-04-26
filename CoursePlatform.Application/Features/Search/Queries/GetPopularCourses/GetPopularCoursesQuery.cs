using CoursePlatform.Application.Features.Courses.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Search.Queries.GetPopularCourses;

public record GetPopularCoursesQuery(int Take = 10)
    : IRequest<IReadOnlyList<CourseSummaryDto>>;
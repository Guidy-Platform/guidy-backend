using CoursePlatform.Application.Features.Courses.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Search.Queries.GetTrendingCourses;

public record GetTrendingCoursesQuery(int Take = 10)
    : IRequest<IReadOnlyList<CourseSummaryDto>>;
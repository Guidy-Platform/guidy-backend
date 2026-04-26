using CoursePlatform.Application.Features.Courses.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Search.Queries.GetRelatedCourses;

public record GetRelatedCoursesQuery(int CourseId)
    : IRequest<IReadOnlyList<CourseSummaryDto>>;
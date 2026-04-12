using CoursePlatform.Application.Features.Courses.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Courses.Queries.GetPendingCourses;

public record GetPendingCoursesQuery : IRequest<IReadOnlyList<CourseSummaryDto>>;
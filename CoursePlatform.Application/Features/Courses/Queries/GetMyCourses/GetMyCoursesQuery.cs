using CoursePlatform.Application.Features.Courses.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Courses.Queries.GetMyCourses;

public record GetMyCoursesQuery : IRequest<IReadOnlyList<CourseSummaryDto>>;
using CoursePlatform.Application.Common.Models;
using CoursePlatform.Application.Features.Courses.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Courses.Queries.GetPublishedCourses;

public record GetPublishedCoursesQuery(CourseQueryParams Params)
    : IRequest<Pagination<CourseSummaryDto>>;
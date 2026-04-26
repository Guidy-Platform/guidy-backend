using CoursePlatform.Application.Features.Courses.DTOs;
using CoursePlatform.Application.Features.Search.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Search.Queries.SearchCourses;

public record SearchCoursesQuery(CourseQueryParams Params)
    : IRequest<SearchResultDto>;
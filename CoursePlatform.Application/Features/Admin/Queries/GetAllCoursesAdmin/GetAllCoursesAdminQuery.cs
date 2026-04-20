using CoursePlatform.Application.Features.Admin.DTOs;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Admin.Queries.GetAllCoursesAdmin;

public record GetAllCoursesAdminQuery(
    CourseStatus? Status = null,
    string? Search = null
) : IRequest<IReadOnlyList<AdminCourseDto>>;
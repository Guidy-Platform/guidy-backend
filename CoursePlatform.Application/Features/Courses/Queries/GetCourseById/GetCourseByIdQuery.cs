using CoursePlatform.Application.Features.Courses.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Courses.Queries.GetCourseById;

public record GetCourseByIdQuery(int Id, bool IsInstructor = false)
    : IRequest<CourseDto>;
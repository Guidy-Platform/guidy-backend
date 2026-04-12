using MediatR;

namespace CoursePlatform.Application.Features.Courses.Commands.ApproveCourse;

public record ApproveCourseCommand(int CourseId) : IRequest<Unit>;
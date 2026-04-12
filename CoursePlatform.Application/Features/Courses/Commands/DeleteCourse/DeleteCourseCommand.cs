using MediatR;

namespace CoursePlatform.Application.Features.Courses.Commands.DeleteCourse;

public record DeleteCourseCommand(int Id) : IRequest<Unit>;
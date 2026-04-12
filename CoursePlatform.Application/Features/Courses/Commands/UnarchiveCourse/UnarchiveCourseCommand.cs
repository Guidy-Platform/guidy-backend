using MediatR;

namespace CoursePlatform.Application.Features.Courses.Commands.UnarchiveCourse;

public record UnarchiveCourseCommand(int CourseId) : IRequest<Unit>;
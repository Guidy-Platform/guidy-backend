using MediatR;

namespace CoursePlatform.Application.Features.Courses.Commands.ArchiveCourse;

public record ArchiveCourseCommand(int CourseId) : IRequest<Unit>;
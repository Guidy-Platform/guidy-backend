using MediatR;

namespace CoursePlatform.Application.Features.Courses.Commands.RejectCourse;

public record RejectCourseCommand(
    int CourseId,
    string Reason
) : IRequest<Unit>;
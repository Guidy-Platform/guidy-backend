using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Courses.Commands.RejectCourse;

public class RejectCourseCommandHandler
    : IRequestHandler<RejectCourseCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly INotificationService _notificationService;

    public RejectCourseCommandHandler(IUnitOfWork uow, INotificationService notificationService)
    {
        _uow = uow;
        _notificationService = notificationService;
    }

    public async Task<Unit> Handle(
        RejectCourseCommand request, CancellationToken ct)
    {
        var course = await _uow.Repository<Course>()
                               .GetByIdAsync(request.CourseId, ct)
            ?? throw new NotFoundException("Course", request.CourseId);

        if (course.Status != CourseStatus.UnderReview)
            throw new BadRequestException(
                "Only courses under review can be rejected.");

        course.Status = CourseStatus.Rejected;
        course.RejectionReason = request.Reason;

        _uow.Repository<Course>().Update(course);
        await _uow.CompleteAsync(ct);

        await _notificationService.SendAsync(
            userId: course.InstructorId,
            title: "Course Needs Changes",
            message: $"Your course '{course.Title}' requires changes: {request.Reason}",
            type: NotificationType.CourseRejected,
            actionUrl: $"/instructor/courses/{course.Id}");

        return Unit.Value;
    }
}
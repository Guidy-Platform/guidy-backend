using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Courses.Commands.ApproveCourse;

public class ApproveCourseCommandHandler
    : IRequestHandler<ApproveCourseCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly ICacheService _cache;
    private readonly INotificationService _notificationService;

    public ApproveCourseCommandHandler(IUnitOfWork uow, ICacheService cache, INotificationService notificationService)
    {
        _uow = uow;
        _cache = cache;
        _notificationService = notificationService;
    }

    public async Task<Unit> Handle(
        ApproveCourseCommand request, CancellationToken ct)
    {
        var course = await _uow.Repository<Course>()
                               .GetByIdAsync(request.CourseId, ct)
            ?? throw new NotFoundException("Course", request.CourseId);

        if (course.Status != CourseStatus.UnderReview)
            throw new BadRequestException(
                "Only courses under review can be approved.");

        course.Status = CourseStatus.Published;

        _uow.Repository<Course>().Update(course);
        await _uow.CompleteAsync(ct);

        await _notificationService.SendAsync(
            userId: course.InstructorId,
            title: "Course Approved!",
            message: $"Your course '{course.Title}' has been approved and is now published.",
            type: NotificationType.CoursePublished,
            actionUrl: $"/courses/{course.Id}");

        await _cache.RemoveByPrefixAsync("courses:published:", ct);

        return Unit.Value;
    }
}
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Courses.Commands.SubmitCourseForReview;

public class SubmitCourseForReviewCommandHandler
    : IRequestHandler<SubmitCourseForReviewCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public SubmitCourseForReviewCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(
        SubmitCourseForReviewCommand request, CancellationToken ct)
    {
        var course = await _uow.Repository<Course>()
                               .GetByIdAsync(request.CourseId, ct)
            ?? throw new NotFoundException("Course", request.CourseId);

        if (course.InstructorId != _currentUser.UserId)
            throw new ForbiddenException();

        // بس Draft أو Rejected يتقدم للـ review
        if (course.Status is not (CourseStatus.Draft or CourseStatus.Rejected))
            throw new BadRequestException(
                $"Only Draft or Rejected courses can be submitted for review. " +
                $"Current status: '{course.Status}'.");

        course.Status = CourseStatus.UnderReview;
        course.RejectionReason = null;

        _uow.Repository<Course>().Update(course);
        await _uow.CompleteAsync(ct);

        return Unit.Value;
    }
}
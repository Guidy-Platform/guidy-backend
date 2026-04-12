using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Courses.Commands.RejectCourse;

public class RejectCourseCommandHandler
    : IRequestHandler<RejectCourseCommand, Unit>
{
    private readonly IUnitOfWork _uow;

    public RejectCourseCommandHandler(IUnitOfWork uow)
        => _uow = uow;

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

        return Unit.Value;
    }
}
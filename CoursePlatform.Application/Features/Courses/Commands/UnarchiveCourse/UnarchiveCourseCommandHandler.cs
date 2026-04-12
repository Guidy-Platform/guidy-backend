using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Courses.Commands.UnarchiveCourse;

public class UnarchiveCourseCommandHandler
    : IRequestHandler<UnarchiveCourseCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cache;

    public UnarchiveCourseCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser,
        ICacheService cache)
    {
        _uow = uow;
        _currentUser = currentUser;
        _cache = cache;
    }

    public async Task<Unit> Handle(
        UnarchiveCourseCommand request, CancellationToken ct)
    {
        var course = await _uow.Repository<Course>()
                               .GetByIdAsync(request.CourseId, ct)
            ?? throw new NotFoundException("Course", request.CourseId);

        var isAdmin = _currentUser.Roles.Contains("Admin");
        var isOwner = course.InstructorId == _currentUser.UserId;

        if (!isAdmin && !isOwner)
            throw new ForbiddenException();

        if (course.Status != CourseStatus.Archived)
            throw new BadRequestException(
                "Only archived courses can be unarchived.");

        // دايماً يرجع Draft — يحتاج review تاني
        course.Status = CourseStatus.Draft;

        _uow.Repository<Course>().Update(course);
        await _uow.CompleteAsync(ct);

        await _cache.RemoveAsync($"courses:detail:{request.CourseId}", ct);

        return Unit.Value;
    }
}
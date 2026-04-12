using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Courses.Commands.ArchiveCourse;

public class ArchiveCourseCommandHandler
    : IRequestHandler<ArchiveCourseCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cache;

    public ArchiveCourseCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser,
        ICacheService cache)
    {
        _uow = uow;
        _currentUser = currentUser;
        _cache = cache;
    }

    public async Task<Unit> Handle(
        ArchiveCourseCommand request, CancellationToken ct)
    {
        var course = await _uow.Repository<Course>()
                               .GetByIdAsync(request.CourseId, ct)
            ?? throw new NotFoundException("Course", request.CourseId);

        var isAdmin = _currentUser.Roles.Contains("Admin");
        var isOwner = course.InstructorId == _currentUser.UserId;

        if (!isAdmin && !isOwner)
            throw new ForbiddenException();

        if (course.Status != CourseStatus.Published)
            throw new BadRequestException(
                "Only published courses can be archived.");

        course.Status = CourseStatus.Archived;

        _uow.Repository<Course>().Update(course);
        await _uow.CompleteAsync(ct);

        await _cache.RemoveByPrefixAsync("courses:published:", ct);
        await _cache.RemoveAsync($"courses:detail:{request.CourseId}", ct);

        return Unit.Value;
    }
}
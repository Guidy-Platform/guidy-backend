using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Courses.Commands.DeleteCourse;

public class DeleteCourseCommandHandler
    : IRequestHandler<DeleteCourseCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cache;

    public DeleteCourseCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser,
        ICacheService cache)
    {
        _uow = uow;
        _currentUser = currentUser;
        _cache = cache;
    }

    public async Task<Unit> Handle(
        DeleteCourseCommand request, CancellationToken ct)
    {
        var course = await _uow.Repository<Course>()
                               .GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Course", request.Id);

        if (course.InstructorId != _currentUser.UserId)
            throw new ForbiddenException(
                "You do not have permission to delete this course.");

        // مينفعش تحذف Published course مباشرة — لازم تـ Archive الأول
        if (course.Status == CourseStatus.Published)
            throw new BadRequestException(
                "Cannot delete a published course. Archive it first.");

        _uow.Repository<Course>().Delete(course);
        await _uow.CompleteAsync(ct);

        await _cache.RemoveByPrefixAsync("courses:published:", ct);
        await _cache.RemoveAsync($"courses:detail:{request.Id}", ct);

        return Unit.Value;
    }
}
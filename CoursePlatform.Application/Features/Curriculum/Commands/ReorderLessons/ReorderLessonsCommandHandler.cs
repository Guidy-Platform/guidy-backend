using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Curriculum.Helpers;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Curriculum.Commands.ReorderLessons;

public class ReorderLessonsCommandHandler
    : IRequestHandler<ReorderLessonsCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public ReorderLessonsCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(
        ReorderLessonsCommand request, CancellationToken ct)
    {
        await CurriculumGuard.GetCourseAndValidateOwnershipAsync(
            request.CourseId, _uow, _currentUser, ct);

        foreach (var item in request.Items)
        {
            var lesson = await _uow.Repository<Lesson>()
                                   .GetByIdAsync(item.LessonId, ct)
                ?? throw new NotFoundException("Lesson", item.LessonId);

            if (lesson.SectionId != request.SectionId)
                throw new ForbiddenException(
                    $"Lesson {item.LessonId} does not belong to this section.");

            lesson.Order = item.Order;
            _uow.Repository<Lesson>().Update(lesson);
        }

        await _uow.CompleteAsync(ct);
        return Unit.Value;
    }
}
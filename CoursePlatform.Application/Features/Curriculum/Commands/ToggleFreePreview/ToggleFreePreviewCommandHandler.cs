using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Curriculum.Helpers;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Curriculum.Commands.ToggleFreePreview;

public class ToggleFreePreviewCommandHandler
    : IRequestHandler<ToggleFreePreviewCommand, bool>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public ToggleFreePreviewCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(
        ToggleFreePreviewCommand request, CancellationToken ct)
    {
        await CurriculumGuard.GetCourseAndValidateOwnershipAsync(
            request.CourseId, _uow, _currentUser, ct);

        var lesson = await _uow.Repository<Lesson>()
                               .GetByIdAsync(request.LessonId, ct)
            ?? throw new NotFoundException("Lesson", request.LessonId);

        if (lesson.SectionId != request.SectionId)
            throw new ForbiddenException(
                "Lesson does not belong to this section.");

        lesson.IsFreePreview = !lesson.IsFreePreview;

        _uow.Repository<Lesson>().Update(lesson);
        await _uow.CompleteAsync(ct);

        return lesson.IsFreePreview;
    }
}
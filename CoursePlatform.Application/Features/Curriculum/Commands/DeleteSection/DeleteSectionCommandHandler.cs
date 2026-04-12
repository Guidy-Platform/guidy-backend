using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Curriculum.Helpers;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Curriculum.Commands.DeleteSection;

public class DeleteSectionCommandHandler
    : IRequestHandler<DeleteSectionCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public DeleteSectionCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(
        DeleteSectionCommand request, CancellationToken ct)
    {
        await CurriculumGuard.GetCourseAndValidateOwnershipAsync(
            request.CourseId, _uow, _currentUser, ct);

        var section = await _uow.Repository<Section>()
                                .GetByIdAsync(request.SectionId, ct)
            ?? throw new NotFoundException("Section", request.SectionId);

        if (section.CourseId != request.CourseId)
            throw new ForbiddenException(
                "Section does not belong to this course.");

        _uow.Repository<Section>().Delete(section);
        await _uow.CompleteAsync(ct);

        return Unit.Value;
    }
}
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Curriculum.Helpers;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Curriculum.Commands.ReorderSections;

public class ReorderSectionsCommandHandler
    : IRequestHandler<ReorderSectionsCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public ReorderSectionsCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(
        ReorderSectionsCommand request, CancellationToken ct)
    {
        await CurriculumGuard.GetCourseAndValidateOwnershipAsync(
            request.CourseId, _uow, _currentUser, ct);

        foreach (var item in request.Items)
        {
            var section = await _uow.Repository<Section>()
                                    .GetByIdAsync(item.SectionId, ct)
                ?? throw new NotFoundException("Section", item.SectionId);

            if (section.CourseId != request.CourseId)
                throw new ForbiddenException(
                    $"Section {item.SectionId} does not belong to this course.");

            section.Order = item.Order;
            _uow.Repository<Section>().Update(section);
        }

        await _uow.CompleteAsync(ct);
        return Unit.Value;
    }
}
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Curriculum.Helpers;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Resources.Commands.DeleteResource;

public class DeleteResourceCommandHandler
    : IRequestHandler<DeleteResourceCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;

    public DeleteResourceCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser,
        IFileStorageService fileStorage)
    {
        _uow = uow;
        _currentUser = currentUser;
        _fileStorage = fileStorage;
    }

    public async Task<Unit> Handle(
        DeleteResourceCommand request, CancellationToken ct)
    {
        // 1. Ownership check
        await CurriculumGuard.GetCourseAndValidateOwnershipAsync(
            request.CourseId, _uow, _currentUser, ct);

        // select the resource
        var resource = await _uow.Repository<Resource>()
                                 .GetByIdAsync(request.ResourceId, ct)
            ?? throw new NotFoundException("Resource", request.ResourceId);

        // check if the resource belongs to the specified lesson
        if (resource.LessonId != request.LessonId)
            throw new ForbiddenException(
                "Resource does not belong to this lesson.");

        // delete the file from storage
        await _fileStorage.DeleteAsync(resource.FileUrl, ct);

        // Delete the resource from the database
        _uow.Repository<Resource>().Delete(resource);
        await _uow.CompleteAsync(ct);

        return Unit.Value;
    }
}
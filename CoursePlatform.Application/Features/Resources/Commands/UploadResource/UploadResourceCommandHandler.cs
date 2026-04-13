using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Curriculum.Helpers;
using CoursePlatform.Application.Features.Resources.DTOs;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Resources.Commands.UploadResource;

public class UploadResourceCommandHandler
    : IRequestHandler<UploadResourceCommand, ResourceDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;
    private readonly IFileTypeValidator _fileValidator;  // ← Interface

    public UploadResourceCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser,
        IFileStorageService fileStorage,
        IFileTypeValidator fileValidator)  // ← Interface
    {
        _uow = uow;
        _currentUser = currentUser;
        _fileStorage = fileStorage;
        _fileValidator = fileValidator;
    }

    public async Task<ResourceDto> Handle(
        UploadResourceCommand request, CancellationToken ct)
    {
        // 1. Ownership check
        await CurriculumGuard.GetCourseAndValidateOwnershipAsync(
            request.CourseId, _uow, _currentUser, ct);

        // 2. Lesson check
        var lesson = await _uow.Repository<Lesson>()
                               .GetByIdAsync(request.LessonId, ct)
            ?? throw new NotFoundException("Lesson", request.LessonId);

        if (lesson.SectionId != request.SectionId)
            throw new ForbiddenException(
                "Lesson does not belong to this section.");

        // 3. Extension validation
        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();

        if (!_fileValidator.IsAllowedExtension(extension))
            throw new BadRequestException(
                $"File type '{extension}' is not allowed. " +
                $"Allowed: {string.Join(", ", _fileValidator.GetAllowedExtensions())}");

        // 4. Content-Type validation
        if (!_fileValidator.IsAllowedContentType(extension, request.ContentType))
            throw new BadRequestException(
                $"Content type '{request.ContentType}' does not match '{extension}'.");

        // 5. Magic bytes validation
        var isValidSignature = await _fileValidator
            .IsValidFileSignatureAsync(request.FileStream, extension, ct);

        if (!isValidSignature)
            throw new BadRequestException(
                "File content does not match its extension.");

        // 6. Unique filename
        var uniqueFileName = $"{Guid.NewGuid():N}{extension}";

        // 7. Save file
        var fileUrl = await _fileStorage.SaveAsync(
            request.FileStream, uniqueFileName, "resources", ct);

        // 8. Save metadata
        var resource = new Resource
        {
            LessonId = request.LessonId,
            Title = request.Title,
            FileUrl = fileUrl,
            FileName = request.FileName,
            FileType = _fileValidator.GetFileType(extension),
            FileSize = request.FileSize
        };

        await _uow.Repository<Resource>().AddAsync(resource, ct);
        await _uow.CompleteAsync(ct);

        return new ResourceDto
        {
            Id = resource.Id,
            Title = resource.Title,
            FileUrl = resource.FileUrl,
            FileName = resource.FileName,
            FileType = resource.FileType,
            FileSize = resource.FileSize,
            FileSizeFormatted = resource.FileSizeFormatted,
            LessonId = resource.LessonId,
            CreatedAt = resource.CreatedAt
        };
    }
}
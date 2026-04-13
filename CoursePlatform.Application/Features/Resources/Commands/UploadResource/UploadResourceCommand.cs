using CoursePlatform.Application.Features.Resources.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Resources.Commands.UploadResource;

public record UploadResourceCommand(
    int LessonId,
    int SectionId,
    int CourseId,
    string Title,
    Stream FileStream,
    string FileName,
    string ContentType,
    long FileSize
) : IRequest<ResourceDto>;
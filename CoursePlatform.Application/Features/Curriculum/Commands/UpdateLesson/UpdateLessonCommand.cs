using CoursePlatform.Application.Features.Curriculum.DTOs;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Curriculum.Commands.UpdateLesson;

public record UpdateLessonCommand(
    int LessonId,
    int SectionId,
    int CourseId,
    string Title,
    string? Description,
    string? VideoUrl,
    int DurationInSeconds,
    LessonType Type,
    bool IsFreePreview
) : IRequest<LessonDto>;
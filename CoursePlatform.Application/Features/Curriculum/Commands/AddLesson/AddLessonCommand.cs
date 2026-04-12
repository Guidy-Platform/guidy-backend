using CoursePlatform.Application.Features.Curriculum.DTOs;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Curriculum.Commands.AddLesson;

public record AddLessonCommand(
    int CourseId,
    int SectionId,
    string Title,
    string? Description,
    string? VideoUrl,
    int DurationInSeconds,
    LessonType Type,
    bool IsFreePreview
) : IRequest<LessonDto>;
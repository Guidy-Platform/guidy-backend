using CoursePlatform.Application.Features.Curriculum.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Curriculum.Commands.UpdateSection;

public record UpdateSectionCommand(
    int SectionId,
    int CourseId,
    string Title
) : IRequest<SectionDto>;
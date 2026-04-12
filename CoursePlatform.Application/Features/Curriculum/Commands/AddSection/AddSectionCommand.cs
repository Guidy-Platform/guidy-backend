using CoursePlatform.Application.Features.Curriculum.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Curriculum.Commands.AddSection;

public record AddSectionCommand(
    int CourseId,
    string Title
) : IRequest<SectionDto>;
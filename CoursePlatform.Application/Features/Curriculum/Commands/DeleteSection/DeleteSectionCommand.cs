using MediatR;

namespace CoursePlatform.Application.Features.Curriculum.Commands.DeleteSection;

public record DeleteSectionCommand(int SectionId, int CourseId) : IRequest<Unit>;
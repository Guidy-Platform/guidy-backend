using CoursePlatform.Application.Features.Resources.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Resources.Queries.GetLessonResources;

public record GetLessonResourcesQuery(
    int LessonId,
    int SectionId,
    int CourseId
) : IRequest<IReadOnlyList<ResourceDto>>;
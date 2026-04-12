using CoursePlatform.Application.Features.Curriculum.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Curriculum.Queries.GetCourseCurriculum;

public record GetCourseCurriculumQuery(
    int CourseId,
    bool IncludeAllLessons = false  // false = free preview only  for public endpoint
) : IRequest<CourseCurriculumDto>;
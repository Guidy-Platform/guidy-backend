using CoursePlatform.Application.Features.Progress.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Progress.Queries.GetCourseProgress;

public record GetCourseProgressQuery(int CourseId)
    : IRequest<CourseProgressDto>;
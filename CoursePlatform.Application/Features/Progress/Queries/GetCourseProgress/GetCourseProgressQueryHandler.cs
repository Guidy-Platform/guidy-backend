using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Curriculum.Specifications;
using CoursePlatform.Application.Features.Enrollments.Specifications;
using CoursePlatform.Application.Features.Progress.DTOs;
using CoursePlatform.Application.Features.Progress.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Progress.Queries.GetCourseProgress;

public class GetCourseProgressQueryHandler
    : IRequestHandler<GetCourseProgressQuery, CourseProgressDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetCourseProgressQueryHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<CourseProgressDto> Handle(
        GetCourseProgressQuery request, CancellationToken ct)
    {
        var studentId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        // check if the student is enrolled in the course
        var enrollmentSpec = new EnrollmentByStudentAndCourseSpec(
            studentId, request.CourseId);
        var isEnrolled = await _uow.Repository<Enrollment>()
                                   .AnyAsync(enrollmentSpec, ct);
        if (!isEnrolled)
            throw new ForbiddenException(
                "You are not enrolled in this course.");

        // get the course with all sections and lessons
        var courseSpec = new CourseCurriculumSpec(request.CourseId);
        var course = await _uow.Repository<Course>()
                                   .GetEntityWithSpecAsync(courseSpec, ct)
            ?? throw new NotFoundException("Course", request.CourseId);

        // get all lesson progresses for the student in this course
        var progressSpec = new LessonProgressByStudentAndCourseSpec(
            studentId, request.CourseId);
        var progresses = await _uow.Repository<LessonProgress>()
                                   .GetAllWithSpecAsync(progressSpec, ct);

        var completedLessonIds = progresses
            .ToDictionary(p => p.LessonId, p => p.CompletedAt);

        var sectionDtos = course.Sections
            .OrderBy(s => s.Order)
            .Select(section =>
            {
                var lessons = section.Lessons
                    .OrderBy(l => l.Order)
                    .Select(lesson => new LessonProgressDto
                    {
                        LessonId = lesson.Id,
                        LessonTitle = lesson.Title,
                        IsCompleted = completedLessonIds.ContainsKey(lesson.Id),
                        CompletedAt = completedLessonIds.TryGetValue(
                                          lesson.Id, out var completedAt)
                                      ? completedAt
                                      : null
                    }).ToList();

                return new SectionProgressDto
                {
                    SectionId = section.Id,
                    SectionTitle = section.Title,
                    TotalLessons = lessons.Count,
                    CompletedLessons = lessons.Count(l => l.IsCompleted),
                    Lessons = lessons
                };
            }).ToList();

        var totalLessons = sectionDtos.Sum(s => s.TotalLessons);
        var completedLessons = sectionDtos.Sum(s => s.CompletedLessons);
        var progressPercent = totalLessons > 0
            ? Math.Round((double)completedLessons / totalLessons * 100, 1)
            : 0;

        return new CourseProgressDto
        {
            CourseId = course.Id,
            CourseTitle = course.Title,
            TotalLessons = totalLessons,
            CompletedLessons = completedLessons,
            ProgressPercent = progressPercent,
            IsCompleted = progressPercent >= 100,
            Sections = sectionDtos
        };
    }
}
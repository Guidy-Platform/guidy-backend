using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Curriculum.Specifications;
using CoursePlatform.Application.Features.Enrollments.Specifications;
using CoursePlatform.Application.Features.Progress.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Progress.Commands.MarkLessonComplete;

public class MarkLessonCompleteCommandHandler
    : IRequestHandler<MarkLessonCompleteCommand, MarkLessonCompleteResult>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public MarkLessonCompleteCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<MarkLessonCompleteResult> Handle(
        MarkLessonCompleteCommand request, CancellationToken ct)
    {
        var studentId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        // 1. check Enrollment
        var enrollmentSpec = new EnrollmentByStudentAndCourseSpec(
            studentId, request.CourseId);
        var isEnrolled = await _uow.Repository<Enrollment>()
                                   .AnyAsync(enrollmentSpec, ct);
        if (!isEnrolled)
            throw new ForbiddenException("You are not enrolled in this course.");

        // 2.get Lesson
        var lesson = await _uow.Repository<Lesson>()
                               .GetByIdAsync(request.LessonId, ct)
            ?? throw new NotFoundException("Lesson", request.LessonId);

        var section = await _uow.Repository<Section>()
                                .GetByIdAsync(lesson.SectionId, ct);

        if (section?.CourseId != request.CourseId)
            throw new BadRequestException(
                "Lesson does not belong to this course.");

        // 3.create or update LessonProgress
        var existingSpec = new LessonProgressByStudentAndLessonSpec(
            studentId, request.LessonId);
        var progress = await _uow.Repository<LessonProgress>()
                                 .GetEntityWithSpecAsync(existingSpec, ct);

        var alreadyCompleted = progress?.IsCompleted ?? false;

        if (!alreadyCompleted)
        {
            if (progress is null)
            {
                if (lesson.Type == LessonType.Video)
                    throw new BadRequestException(
                        "Video lessons must be watched before marking as complete. " +
                        "Use the watch-time endpoint.");

                progress = new LessonProgress
                {
                    StudentId = studentId,
                    LessonId = request.LessonId,
                    CourseId = request.CourseId,
                    WatchedSeconds = lesson.DurationInSeconds,
                    IsCompleted = true,
                    CompletedAt = DateTime.UtcNow,
                    LastWatchedAt = DateTime.UtcNow
                };
                await _uow.Repository<LessonProgress>().AddAsync(progress, ct);
            }
            else
            {
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.UtcNow;
                _uow.Repository<LessonProgress>().Update(progress);
            }

            await _uow.CompleteAsync(ct);
        }

        // 4. احسب الـ Progress
        var courseSpec = new CourseCurriculumSpec(request.CourseId);
        var course = await _uow.Repository<Course>()
                                   .GetEntityWithSpecAsync(courseSpec, ct);

        var totalLessons = course!.Sections
            .SelectMany(s => s.Lessons)
            .Count();

        var completedCountSpec = new LessonProgressByStudentAndCourseSpec(
            studentId, request.CourseId);
        var completedCount = await _uow.Repository<LessonProgress>()
                                       .CountAsync(completedCountSpec, ct);

        var progressPercent = totalLessons > 0
            ? Math.Round((double)completedCount / totalLessons * 100, 1)
            : 0;

        return new MarkLessonCompleteResult(
            LessonId: request.LessonId,
            AlreadyCompleted: alreadyCompleted,
            NewProgressPercent: progressPercent,
            CourseCompleted: progressPercent >= 100
        );
    }
}
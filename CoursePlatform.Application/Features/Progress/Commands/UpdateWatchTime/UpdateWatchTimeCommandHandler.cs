using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Enrollments.Specifications;
using CoursePlatform.Application.Features.Progress.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Progress.Commands.UpdateWatchTime;

public class UpdateWatchTimeCommandHandler
    : IRequestHandler<UpdateWatchTimeCommand, UpdateWatchTimeResult>
{
    private const double CompletionThreshold = 0.8; // 80%

    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public UpdateWatchTimeCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<UpdateWatchTimeResult> Handle(
        UpdateWatchTimeCommand request, CancellationToken ct)
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

        // create or update the LessonProgress for the student and lesson
        var progressSpec = new LessonProgressByStudentAndLessonSpec(
            studentId, request.LessonId);
        var progress = await _uow.Repository<LessonProgress>()
                                 .GetEntityWithSpecAsync(progressSpec, ct);

        var justCompleted = false;

        if (progress is null)
        {
            // check if first time, create new progress
            progress = new LessonProgress
            {
                StudentId = studentId,
                LessonId = request.LessonId,
                CourseId = request.CourseId,
                WatchedSeconds = request.WatchedSeconds,
                LastWatchedAt = DateTime.UtcNow
            };
            await _uow.Repository<LessonProgress>().AddAsync(progress, ct);
        }
        else
        {

            // update only if the new watched seconds is greater than the existing one
            if (request.WatchedSeconds > progress.WatchedSeconds)
                progress.WatchedSeconds = request.WatchedSeconds;

            progress.LastWatchedAt = DateTime.UtcNow;
            _uow.Repository<LessonProgress>().Update(progress);
        }

        // if greater than or equal to 80% and not already completed, mark as completed
        var watchedPercent = request.TotalSeconds > 0
            ? (double)progress.WatchedSeconds / request.TotalSeconds
            : 0;

        if (watchedPercent >= CompletionThreshold && !progress.IsCompleted)
        {
            progress.IsCompleted = true;
            progress.CompletedAt = DateTime.UtcNow;
            justCompleted = true;
        }

        await _uow.CompleteAsync(ct);

        return new UpdateWatchTimeResult(
            LessonId: request.LessonId,
            WatchedSeconds: progress.WatchedSeconds,
            WatchedPercent: Math.Round(watchedPercent * 100, 1),
            JustCompleted: justCompleted
        );
    }
}
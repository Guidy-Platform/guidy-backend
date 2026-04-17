using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Curriculum.Specifications;
using CoursePlatform.Application.Features.Enrollments.DTOs;
using CoursePlatform.Application.Features.Enrollments.Specifications;
using CoursePlatform.Application.Features.Progress.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Enrollments.Queries.GetEnrolledCourseDetails;

public class GetEnrolledCourseDetailsQueryHandler
    : IRequestHandler<GetEnrolledCourseDetailsQuery, EnrollmentDetailsDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetEnrolledCourseDetailsQueryHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<EnrollmentDetailsDto> Handle(
        GetEnrolledCourseDetailsQuery request, CancellationToken ct)
    {
        var studentId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        // تحقق من الـ Enrollment
        var enrollmentSpec = new EnrollmentByStudentAndCourseSpec(
            studentId, request.CourseId);
        var enrollment = await _uow.Repository<Enrollment>()
                                   .GetEntityWithSpecAsync(enrollmentSpec, ct)
            ?? throw new ForbiddenException(
                "You are not enrolled in this course.");

        // جيب الـ Course مع الـ Curriculum
        var courseSpec = new CourseCurriculumSpec(request.CourseId);
        var course = await _uow.Repository<Course>()
                                   .GetEntityWithSpecAsync(courseSpec, ct)
            ?? throw new NotFoundException("Course", request.CourseId);

        // جيب الـ Progress
        var progressSpec = new LessonProgressByStudentAndCourseSpec(
            studentId, request.CourseId);
        var progresses = await _uow.Repository<LessonProgress>()
                                   .GetAllWithSpecAsync(progressSpec, ct);

        var completedLessonIds = progresses
            .Where(p => p.IsCompleted)      
            .ToDictionary(p => p.LessonId, p => p.CompletedAt);

        var totalLessons = course.Sections
            .SelectMany(s => s.Lessons)
            .Count();

        var completedLessons = completedLessonIds.Count;

        var progressPercent = totalLessons > 0
            ? Math.Round((double)completedLessons / totalLessons * 100, 1)
            : 0;

        return new EnrollmentDetailsDto
        {
            CourseId = course.Id,
            CourseTitle = course.Title,
            ThumbnailUrl = course.ThumbnailUrl,
            InstructorName = course.Instructor?.FullName ?? string.Empty,
            EnrolledAt = enrollment.EnrolledAt,
            ProgressPercent = progressPercent,
            CompletedLessons = completedLessons,
            TotalLessons = totalLessons,
            IsCompleted = progressPercent >= 100
        };
    }
}
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Certificates.DTOs;
using CoursePlatform.Application.Features.Certificates.Specifications;
using CoursePlatform.Application.Features.Curriculum.Specifications;
using CoursePlatform.Application.Features.Enrollments.Specifications;
using CoursePlatform.Application.Features.Progress.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Certificates.Commands.IssueCertificate;

public class IssueCertificateCommandHandler
    : IRequestHandler<IssueCertificateCommand, CertificateDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly IUserRepository _userRepo;   
    private readonly INotificationService _notificationService;

    public IssueCertificateCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser,
        IUserRepository userRepo,
        INotificationService notificationService)
    {
        _uow = uow;
        _currentUser = currentUser;
        _userRepo = userRepo;
        _notificationService = notificationService;
    }

    public async Task<CertificateDto> Handle(
        IssueCertificateCommand request, CancellationToken ct)
    {
        var studentId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        // 1. check enrollment
        var enrollmentSpec = new EnrollmentByStudentAndCourseSpec(
            studentId, request.CourseId);
        var isEnrolled = await _uow.Repository<Enrollment>()
                                   .AnyAsync(enrollmentSpec, ct);
        if (!isEnrolled)
            throw new ForbiddenException(
                "You are not enrolled in this course.");

        // 2. check curriculum completion
        var courseSpec = new CourseCurriculumSpec(request.CourseId);
        var course = await _uow.Repository<Course>()
                                   .GetEntityWithSpecAsync(courseSpec, ct)
            ?? throw new NotFoundException("Course", request.CourseId);

        var totalLessons = course.Sections
            .SelectMany(s => s.Lessons)
            .Count();

        var progressSpec = new LessonProgressByStudentAndCourseSpec(
            studentId, request.CourseId);
        var completedCount = await _uow.Repository<LessonProgress>()
                                       .CountAsync(progressSpec, ct);

        if (totalLessons == 0 || completedCount < totalLessons)
            throw new BadRequestException(
                $"You must complete all lessons to get a certificate. " +
                $"Progress: {completedCount}/{totalLessons}");

        // 3. Idempotent check
        var existingSpec = new CertificateByStudentAndCourseSpec(
            studentId, request.CourseId);
        var existing = await _uow.Repository<Certificate>()
                                 .GetEntityWithSpecAsync(existingSpec, ct);

        if (existing is not null)
            return MapToDto(existing, _currentUser.BaseUrl);  // ← من ICurrentUserService

        // 4. Get Student Info
        var student = await _userRepo.GetByIdAsync(studentId, ct)  // ← IUserRepository
            ?? throw new NotFoundException("Student", studentId);

        // 5. Create Certificate
        var certificate = new Certificate
        {
            StudentId = studentId,
            CourseId = request.CourseId,
            VerifyCode = Guid.NewGuid().ToString("N").ToUpper()[..16],
            StudentName = student.FullName,
            CourseName = course.Title,
            InstructorName = course.Instructor?.FullName ?? string.Empty,
            IssuedAt = DateTime.UtcNow
        };

        await _uow.Repository<Certificate>().AddAsync(certificate, ct);
        await _uow.CompleteAsync(ct);

        // في IssueCertificateCommandHandler — بعد CompleteAsync

        await _notificationService.SendAsync(
            userId: studentId,
            title: "Certificate Issued!",
            message: $"Congratulations! Your certificate for '{course.Title}' is ready.",
            type: NotificationType.CertificateIssued,
            actionUrl: $"/certificates/{certificate.Id}");

        return MapToDto(certificate, _currentUser.BaseUrl);
    }

    internal static CertificateDto MapToDto(
        Certificate cert, string baseUrl) => new()
        {
            Id = cert.Id,
            VerifyCode = cert.VerifyCode,
            StudentName = cert.StudentName,
            CourseName = cert.CourseName,
            InstructorName = cert.InstructorName,
            IssuedAt = cert.IssuedAt,
            VerifyUrl = $"{baseUrl}/api/certificates/verify/{cert.VerifyCode}"
        };
}
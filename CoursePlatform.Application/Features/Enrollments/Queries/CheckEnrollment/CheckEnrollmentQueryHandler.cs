using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Enrollments.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Enrollments.Queries.CheckEnrollment;

public class CheckEnrollmentQueryHandler
    : IRequestHandler<CheckEnrollmentQuery, bool>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public CheckEnrollmentQueryHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(
        CheckEnrollmentQuery request, CancellationToken ct)
    {
        var studentId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var spec = new EnrollmentByStudentAndCourseSpec(
            studentId, request.CourseId);

        return await _uow.Repository<Enrollment>().AnyAsync(spec, ct);
    }
}
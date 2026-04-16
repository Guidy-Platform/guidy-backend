using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Enrollments.DTOs;
using CoursePlatform.Application.Features.Enrollments.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Enrollments.Queries.GetMyEnrollments;

public class GetMyEnrollmentsQueryHandler
    : IRequestHandler<GetMyEnrollmentsQuery, IReadOnlyList<EnrollmentDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetMyEnrollmentsQueryHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<EnrollmentDto>> Handle(
        GetMyEnrollmentsQuery request, CancellationToken ct)
    {
        var studentId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var spec = new EnrollmentByStudentSpec(studentId);
        var enrollments = await _uow.Repository<Enrollment>()
                                    .GetAllWithSpecAsync(spec, ct);

        return enrollments.Select(e => new EnrollmentDto
        {
            Id = e.Id,
            CourseId = e.CourseId,
            CourseTitle = e.Course.Title,
            ThumbnailUrl = e.Course.ThumbnailUrl,
            EnrolledAt = e.EnrolledAt
        }).ToList();
    }
}
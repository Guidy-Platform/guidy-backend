using CoursePlatform.Application.Features.Enrollments.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Enrollments.Queries.GetMyEnrollments;

public record GetMyEnrollmentsQuery : IRequest<IReadOnlyList<EnrollmentDto>>;
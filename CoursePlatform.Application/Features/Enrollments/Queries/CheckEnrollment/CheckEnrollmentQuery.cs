using MediatR;

namespace CoursePlatform.Application.Features.Enrollments.Queries.CheckEnrollment;

public record CheckEnrollmentQuery(int CourseId) : IRequest<bool>;
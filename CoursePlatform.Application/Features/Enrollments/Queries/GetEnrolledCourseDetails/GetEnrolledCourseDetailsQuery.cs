using CoursePlatform.Application.Features.Enrollments.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Enrollments.Queries.GetEnrolledCourseDetails;

public record GetEnrolledCourseDetailsQuery(int CourseId)
    : IRequest<EnrollmentDetailsDto>;
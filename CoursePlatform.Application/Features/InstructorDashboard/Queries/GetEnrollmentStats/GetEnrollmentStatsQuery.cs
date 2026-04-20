using CoursePlatform.Application.Features.InstructorDashboard.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.InstructorDashboard.Queries.GetEnrollmentStats;

public record GetEnrollmentStatsQuery(
    int Months = 12
) : IRequest<EnrollmentStatsDto>;
using CoursePlatform.Application.Features.InstructorDashboard.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.InstructorDashboard.Queries.GetDashboardSummary;

public record GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>;
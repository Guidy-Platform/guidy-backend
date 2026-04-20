using CoursePlatform.Application.Features.InstructorDashboard.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.InstructorDashboard.Queries.GetRevenueStats;

public record GetRevenueStatsQuery(
    int Months = 12   
) : IRequest<RevenueStatsDto>;
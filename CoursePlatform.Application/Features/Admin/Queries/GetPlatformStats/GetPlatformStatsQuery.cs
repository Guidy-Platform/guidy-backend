using CoursePlatform.Application.Features.Admin.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Admin.Queries.GetPlatformStats;

public record GetPlatformStatsQuery(int Months = 6)
    : IRequest<PlatformStatsDto>;
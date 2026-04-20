using CoursePlatform.Application.Common.Models;
using CoursePlatform.Application.Features.Admin.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Admin.Queries.GetAllUsers;

public record GetAllUsersQuery(
    string? Search = null,
    bool? IsBanned = null,
    int PageIndex = 1,
    int PageSize = 20
) : IRequest<IReadOnlyList<AdminUserDto>>;
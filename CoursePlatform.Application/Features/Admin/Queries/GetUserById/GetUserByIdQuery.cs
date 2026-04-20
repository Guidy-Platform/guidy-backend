using CoursePlatform.Application.Features.Admin.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Admin.Queries.GetUserById;

public record GetUserByIdQuery(Guid UserId) : IRequest<AdminUserDto>;
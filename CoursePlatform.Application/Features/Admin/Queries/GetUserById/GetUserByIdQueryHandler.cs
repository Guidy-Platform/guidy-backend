using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Features.Admin.DTOs;
using CoursePlatform.Application.Features.Admin.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Admin.Queries.GetUserById;

public class GetUserByIdQueryHandler
    : IRequestHandler<GetUserByIdQuery, AdminUserDto>
{
    private readonly IUserRepository _userRepo;
    private readonly IUnitOfWork _uow;

    public GetUserByIdQueryHandler(
        IUserRepository userRepo,
        IUnitOfWork uow)
    {
        _userRepo = userRepo;
        _uow = uow;
    }

    public async Task<AdminUserDto> Handle(
        GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await _userRepo.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException("User", request.UserId);

        var roles = await _userRepo.GetRolesAsync(user, ct);
        var enrollments = await _uow.Repository<Enrollment>()
            .CountAsync(new EnrollmentByStudentSpec(user.Id), ct);
        var courses = await _uow.Repository<Course>()
            .CountAsync(new CoursesByInstructorCountSpec(user.Id), ct);

        return new AdminUserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            ProfilePicture = user.ProfilePictureUrl,
            IsEmailConfirmed = user.EmailConfirmed,
            IsBanned = user.IsBanned,
            CreatedAt = user.CreatedAt,
            Roles = roles.ToList(),
            EnrollmentsCount = enrollments,
            CoursesCount = courses
        };
    }
}
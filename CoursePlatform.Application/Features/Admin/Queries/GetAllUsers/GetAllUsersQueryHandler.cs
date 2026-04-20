// Application/Features/Admin/Queries/GetAllUsers/GetAllUsersQueryHandler.cs
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Features.Admin.DTOs;
using CoursePlatform.Application.Features.Admin.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Admin.Queries.GetAllUsers;

public class GetAllUsersQueryHandler
    : IRequestHandler<GetAllUsersQuery, IReadOnlyList<AdminUserDto>>
{
    private readonly IUserRepository _userRepo;
    private readonly IUnitOfWork _uow;

    public GetAllUsersQueryHandler(
        IUserRepository userRepo,
        IUnitOfWork uow)
    {
        _userRepo = userRepo;
        _uow = uow;
    }

    public async Task<IReadOnlyList<AdminUserDto>> Handle(
        GetAllUsersQuery request, CancellationToken ct)
    {
        var users = await _userRepo.GetAllAsync(
            request.Search, request.IsBanned, ct);

        var result = new List<AdminUserDto>();

        foreach (var user in users)
        {
            var roles = await _userRepo.GetRolesAsync(user, ct);
            var enrollments = await _uow.Repository<Enrollment>()
                .CountAsync(new EnrollmentByStudentSpec(user.Id), ct);
            var courses = await _uow.Repository<Course>()
                .CountAsync(new CoursesByInstructorCountSpec(user.Id), ct);

            result.Add(new AdminUserDto
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
            });
        }

        // Pagination في الـ memory
        return result
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();
    }
}
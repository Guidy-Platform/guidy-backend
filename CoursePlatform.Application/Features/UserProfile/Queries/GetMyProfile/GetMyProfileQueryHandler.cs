using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.UserProfile.DTOs;
using CoursePlatform.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CoursePlatform.Application.Features.UserProfile.Queries.GetMyProfile;

public class GetMyProfileQueryHandler
    : IRequestHandler<GetMyProfileQuery, UserProfileDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICurrentUserService _currentUser;

    public GetMyProfileQueryHandler(
        UserManager<AppUser> userManager,
        ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public async Task<UserProfileDto> Handle(
        GetMyProfileQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User", userId);

        var roles = await _userManager.GetRolesAsync(user);

        return new UserProfileDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            Email = user.Email!,
            Bio = user.Bio,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Roles = [.. roles],
            CreatedAt = user.CreatedAt
        };
    }
}
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.UserProfile.DTOs;
using CoursePlatform.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CoursePlatform.Application.Features.UserProfile.Commands.UpdateProfile;

public class UpdateProfileCommandHandler
    : IRequestHandler<UpdateProfileCommand, UserProfileDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICurrentUserService _currentUser;

    public UpdateProfileCommandHandler(
        UserManager<AppUser> userManager,
        ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public async Task<UserProfileDto> Handle(
        UpdateProfileCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User", userId);

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Bio = request.Bio;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new BadRequestException(
                string.Join(", ", result.Errors.Select(e => e.Description)));

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
// Application/Features/UserProfile/Commands/DeleteAvatar/DeleteAvatarCommandHandler.cs
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CoursePlatform.Application.Features.UserProfile.Commands.DeleteAvatar;

public class DeleteAvatarCommandHandler
    : IRequestHandler<DeleteAvatarCommand, Unit>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;

    public DeleteAvatarCommandHandler(
        UserManager<AppUser> userManager,
        ICurrentUserService currentUser,
        IFileStorageService fileStorage)
    {
        _userManager = userManager;
        _currentUser = currentUser;
        _fileStorage = fileStorage;
    }

    public async Task<Unit> Handle(
        DeleteAvatarCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User", userId);

        if (string.IsNullOrEmpty(user.ProfilePictureUrl))
            throw new BadRequestException("No avatar to delete.");

        await _fileStorage.DeleteAsync(user.ProfilePictureUrl, ct);

        user.ProfilePictureUrl = null;
        await _userManager.UpdateAsync(user);

        return Unit.Value;
    }
}
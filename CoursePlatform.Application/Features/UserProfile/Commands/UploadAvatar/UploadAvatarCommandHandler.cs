using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.UserProfile.DTOs;
using CoursePlatform.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CoursePlatform.Application.Features.UserProfile.Commands.UploadAvatar;

public class UploadAvatarCommandHandler
    : IRequestHandler<UploadAvatarCommand, UserProfileDto>
{
    private static readonly string[] AllowedExtensions =
        [".jpg", ".jpeg", ".png", ".webp"];

    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB

    private readonly UserManager<AppUser> _userManager;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;

    public UploadAvatarCommandHandler(
        UserManager<AppUser> userManager,
        ICurrentUserService currentUser,
        IFileStorageService fileStorage)
    {
        _userManager = userManager;
        _currentUser = currentUser;
        _fileStorage = fileStorage;
    }

    public async Task<UserProfileDto> Handle(
        UploadAvatarCommand request, CancellationToken ct)
    {
        // 1. Validation
        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            throw new BadRequestException(
                $"Invalid file type. Allowed: {string.Join(", ", AllowedExtensions)}");

        if (request.FileSize > MaxFileSizeBytes)
            throw new BadRequestException("File size cannot exceed 5MB.");

        // 2. جلب الـ user
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User", userId);

        // 3. حذف الصورة القديمة لو موجودة
        if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            await _fileStorage.DeleteAsync(user.ProfilePictureUrl, ct);

        // 4. حفظ الصورة الجديدة
        // نعمل unique filename عشان منعملش overwrite
        var uniqueFileName = $"{userId}{extension}";

        var fileUrl = await _fileStorage.SaveAsync(
            request.FileStream,
            uniqueFileName,
            "avatars",
            ct);

        // 5. تحديث الـ user
        user.ProfilePictureUrl = fileUrl;
        await _userManager.UpdateAsync(user);

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
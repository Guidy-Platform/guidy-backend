using CoursePlatform.Application.Features.UserProfile.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.UserProfile.Commands.UploadAvatar;

public record UploadAvatarCommand(
    Stream FileStream,
    string FileName,
    long FileSize,
    string ContentType
) : IRequest<UserProfileDto>;
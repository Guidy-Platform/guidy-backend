// Application/Features/UserProfile/Commands/UpdateProfile/UpdateProfileCommand.cs
using CoursePlatform.Application.Features.UserProfile.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.UserProfile.Commands.UpdateProfile;

public record UpdateProfileCommand(
    string FirstName,
    string LastName,
    string? Bio
) : IRequest<UserProfileDto>;
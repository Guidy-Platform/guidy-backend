// Application/Features/UserProfile/Commands/DeleteAvatar/DeleteAvatarCommand.cs
using MediatR;

namespace CoursePlatform.Application.Features.UserProfile.Commands.DeleteAvatar;

public record DeleteAvatarCommand : IRequest<Unit>;
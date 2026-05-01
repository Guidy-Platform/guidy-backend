using CoursePlatform.Application.Features.Settings.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Settings.Commands.UpdateSettings;

public record UpdateSettingsCommand(
    PlatformSettingsDto Settings
) : IRequest<PlatformSettingsDto>;
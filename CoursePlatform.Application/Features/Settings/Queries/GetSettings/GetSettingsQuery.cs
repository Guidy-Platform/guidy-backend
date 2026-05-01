using CoursePlatform.Application.Features.Settings.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Settings.Queries.GetSettings;

public record GetSettingsQuery : IRequest<PlatformSettingsDto>;
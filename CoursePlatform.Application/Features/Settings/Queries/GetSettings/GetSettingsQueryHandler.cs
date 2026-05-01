using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Settings.DTOs;
using CoursePlatform.Application.Features.Settings.Specifications;
using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Settings.Queries.GetSettings;

public class GetSettingsQueryHandler
    : IRequestHandler<GetSettingsQuery, PlatformSettingsDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICacheService _cache;

    public GetSettingsQueryHandler(IUnitOfWork uow, ICacheService cache)
    {
        _uow = uow;
        _cache = cache;
    }

    public async Task<PlatformSettingsDto> Handle(
        GetSettingsQuery request, CancellationToken ct)
    {
        const string cacheKey = "platform:settings";

        var cached = await _cache.GetAsync<PlatformSettingsDto>(cacheKey, ct);
        if (cached is not null) return cached;

        var spec = new AllSettingsSpec();
        var settings = await _uow.Repository<PlatformSetting>()
                                 .GetAllWithSpecAsync(spec, ct);

        var dict = settings.ToDictionary(s => s.Key, s => s.Value);

        var dto = new PlatformSettingsDto
        {
            Name = dict.GetValueOrDefault("platform.name", "Guidy Platform"),
            Description = dict.GetValueOrDefault("platform.description", ""),
            LogoUrl = dict.GetValueOrDefault("platform.logoUrl"),
            FaviconUrl = dict.GetValueOrDefault("platform.faviconUrl"),
            Currency = dict.GetValueOrDefault("platform.currency", "USD"),
            Language = dict.GetValueOrDefault("platform.language", "en"),
            ContactEmail = dict.GetValueOrDefault("contact.email"),
            ContactPhone = dict.GetValueOrDefault("contact.phone"),
            ContactAddress = dict.GetValueOrDefault("contact.address"),
            WorkingHours = dict.GetValueOrDefault("contact.workingHours"),
            Facebook = dict.GetValueOrDefault("social.facebook"),
            Twitter = dict.GetValueOrDefault("social.twitter"),
            Instagram = dict.GetValueOrDefault("social.instagram"),
            LinkedIn = dict.GetValueOrDefault("social.linkedin"),
            Youtube = dict.GetValueOrDefault("social.youtube"),
            MetaTitle = dict.GetValueOrDefault("seo.metaTitle"),
            MetaDescription = dict.GetValueOrDefault("seo.metaDescription"),
            AllowRegister = dict.GetValueOrDefault("features.allowRegister", "true") == "true",
            MaintenanceMode = dict.GetValueOrDefault("features.maintenanceMode", "false") == "true",
            AllowGoogleLogin = dict.GetValueOrDefault("features.allowGoogleLogin", "true") == "true",
            AllowSubscription = dict.GetValueOrDefault("features.allowSubscription", "true") == "true"
        };

        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromHours(1), ct);

        return dto;
    }
}
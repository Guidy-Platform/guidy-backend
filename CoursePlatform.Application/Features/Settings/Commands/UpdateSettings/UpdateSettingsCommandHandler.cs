// Application/Features/Settings/Commands/UpdateSettings/UpdateSettingsCommandHandler.cs
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Settings.DTOs;
using CoursePlatform.Application.Features.Settings.Specifications;
using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Settings.Commands.UpdateSettings;

public class UpdateSettingsCommandHandler
    : IRequestHandler<UpdateSettingsCommand, PlatformSettingsDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICacheService _cache;

    public UpdateSettingsCommandHandler(
        IUnitOfWork uow, ICacheService cache)
    {
        _uow = uow;
        _cache = cache;
    }

    public async Task<PlatformSettingsDto> Handle(
        UpdateSettingsCommand request, CancellationToken ct)
    {
        var s = request.Settings;

        var updates = new Dictionary<string, string>
        {
            ["platform.name"] = s.Name,
            ["platform.description"] = s.Description,
            ["platform.logoUrl"] = s.LogoUrl ?? string.Empty,
            ["platform.faviconUrl"] = s.FaviconUrl ?? string.Empty,
            ["platform.currency"] = s.Currency,
            ["platform.language"] = s.Language,
            ["contact.email"] = s.ContactEmail ?? string.Empty,
            ["contact.phone"] = s.ContactPhone ?? string.Empty,
            ["contact.address"] = s.ContactAddress ?? string.Empty,
            ["contact.workingHours"] = s.WorkingHours ?? string.Empty,
            ["social.facebook"] = s.Facebook ?? string.Empty,
            ["social.twitter"] = s.Twitter ?? string.Empty,
            ["social.instagram"] = s.Instagram ?? string.Empty,
            ["social.linkedin"] = s.LinkedIn ?? string.Empty,
            ["social.youtube"] = s.Youtube ?? string.Empty,
            ["seo.metaTitle"] = s.MetaTitle ?? string.Empty,
            ["seo.metaDescription"] = s.MetaDescription ?? string.Empty,
            ["features.allowRegister"] = s.AllowRegister.ToString().ToLower(),
            ["features.maintenanceMode"] = s.MaintenanceMode.ToString().ToLower(),
            ["features.allowGoogleLogin"] = s.AllowGoogleLogin.ToString().ToLower(),
            ["features.allowSubscription"] = s.AllowSubscription.ToString().ToLower()
        };

        var spec = new AllSettingsSpec();
        var existing = await _uow.Repository<PlatformSetting>()
                                 .GetAllWithSpecAsync(spec, ct);
        var dict = existing.ToDictionary(x => x.Key);

        foreach (var (key, value) in updates)
        {
            if (dict.TryGetValue(key, out var setting))
            {
                setting.Value = value;
                _uow.Repository<PlatformSetting>().Update(setting);
            }
            else
            {
                var group = key.Split('.')[0];
                await _uow.Repository<PlatformSetting>().AddAsync(
                    new PlatformSetting
                    {
                        Key = key,
                        Value = value,
                        Group = group
                    }, ct);
            }
        }

        await _uow.CompleteAsync(ct);

        // Invalidate Cache
        await _cache.RemoveAsync("platform:settings", ct);

        return s;
    }
}
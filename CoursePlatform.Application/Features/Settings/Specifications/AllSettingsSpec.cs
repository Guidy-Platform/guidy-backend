using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Settings.Specifications;

public class AllSettingsSpec : BaseSpecification<PlatformSetting>
{
    public AllSettingsSpec() : base(s => true)
    {
        ApplyNoTracking();
    }
}
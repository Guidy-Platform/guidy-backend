using CoursePlatform.Domain.Common;

namespace CoursePlatform.Domain.Entities;

public class PlatformSetting : AuditableEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Group { get; set; }  // "general", "social", "contact"
}
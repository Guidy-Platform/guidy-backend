namespace CoursePlatform.Application.Features.Settings.DTOs;

public class PlatformSettingsDto
{
    // General
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? FaviconUrl { get; set; }
    public string Currency { get; set; } = "USD";
    public string Language { get; set; } = "en";

    // Contact
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactAddress { get; set; }
    public string? WorkingHours { get; set; }

    // Social
    public string? Facebook { get; set; }
    public string? Twitter { get; set; }
    public string? Instagram { get; set; }
    public string? LinkedIn { get; set; }
    public string? Youtube { get; set; }

    // SEO
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }

    // Features
    public bool AllowRegister { get; set; } = true;
    public bool MaintenanceMode { get; set; } = false;
    public bool AllowGoogleLogin { get; set; } = true;
    public bool AllowSubscription { get; set; } = true;
}
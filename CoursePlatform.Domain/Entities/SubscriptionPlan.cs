// Domain/Entities/SubscriptionPlan.cs
using CoursePlatform.Domain.Common;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Domain.Entities;

public class SubscriptionPlan : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public BillingInterval BillingInterval { get; set; }
    public bool IsActive { get; set; } = true;
    public string? StripePriceId { get; set; }  // Stripe Price ID
    public string? StripeProductId { get; set; }  // Stripe Product ID

    // Features
    public bool UnlimitedCourseAccess { get; set; } = true;
    public bool CertificateAccess { get; set; } = true;
}
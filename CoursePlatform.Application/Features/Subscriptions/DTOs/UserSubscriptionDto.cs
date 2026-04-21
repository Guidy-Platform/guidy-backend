namespace CoursePlatform.Application.Features.Subscriptions.DTOs;

public class UserSubscriptionDto
{
    public int Id { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public string BillingInterval { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool AutoRenew { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? CancelledAt { get; set; }
    public int DaysRemaining { get; set; }
    public string? ClientSecret { get; set; }  // for Frontend
}
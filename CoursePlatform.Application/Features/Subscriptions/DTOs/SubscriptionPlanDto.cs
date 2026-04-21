namespace CoursePlatform.Application.Features.Subscriptions.DTOs;

public class SubscriptionPlanDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string BillingInterval { get; set; } = string.Empty;
    public decimal PricePerMonth { get; set; }  // for Annual: 199.99/12
    public int? SavePercent { get; set; }  // for Annual: 44%
    public bool IsActive { get; set; }
}
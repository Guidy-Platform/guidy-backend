namespace CoursePlatform.Domain.Enums;

public enum SubscriptionStatus
{
    Active = 1,
    Cancelled = 2,   // still active until the end of the current period, but will not renew
    Expired = 3,   // period has ended
    PastDue = 4    // renewal payment failed    
}
namespace CoursePlatform.Domain.Enums;

public enum PayoutStatus
{
    Pending = 1,   // Instructor or Admin order created but not yet approved
    Approved = 2,   // Admin accept + Stripe Transfer send
    Rejected = 3,   // Admin reject 
    Failed = 4    // Stripe Transfer failed
}

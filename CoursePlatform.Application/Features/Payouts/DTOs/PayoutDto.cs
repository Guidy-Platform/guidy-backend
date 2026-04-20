namespace CoursePlatform.Application.Features.Payouts.DTOs;

public class PayoutDto
{
    public int Id { get; set; }
    public Guid InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal PlatformFee { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public string? StripeTransferId { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}
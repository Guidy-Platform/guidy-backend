namespace CoursePlatform.Application.Features.Coupons.DTOs;

public class CouponValidationDto
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal DiscountAmount { get; set; }  // بيتحسب على الـ amount
    public decimal FinalAmount { get; set; }
}
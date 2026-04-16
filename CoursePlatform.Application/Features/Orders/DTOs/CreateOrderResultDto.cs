namespace CoursePlatform.Application.Features.Orders.DTOs;

public class CreateOrderResultDto
{
    public int OrderId { get; set; }
    public decimal FinalPrice { get; set; }
    public string ClientSecret { get; set; } = string.Empty;  // for Stripe.js
    public bool IsFree { get; set; }  // if price is 0
}
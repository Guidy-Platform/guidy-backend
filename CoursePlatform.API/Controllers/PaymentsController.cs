// API/Controllers/PaymentsController.cs
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Orders.Commands.HandlePaymentSuccess;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace CoursePlatform.API.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IPaymentService _payment;
    private readonly string _webhookSecret;

    public PaymentsController(ISender sender, IPaymentService payment ,IConfiguration config)
    {
        _sender = sender;
        _payment = payment;
        _webhookSecret = config["Stripe:WebhookSecret"]!;
    }

    [HttpPost("webhook")]
    [AllowAnonymous]
   
    public async Task<IActionResult> Webhook(CancellationToken ct)
    {
        // تأكد إن الـ body seekable
        if (!Request.Body.CanSeek)
        {
            Request.EnableBuffering();
            await Request.Body.DrainAsync(ct);
            Request.Body.Position = 0;
        }

        string payload;
        using (var reader = new StreamReader(
            Request.Body,
            encoding: System.Text.Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true))
        {
            payload = await reader.ReadToEndAsync(ct);
        }

        Request.Body.Position = 0;

        var signature = Request.Headers["Stripe-Signature"].FirstOrDefault() ?? "";

        Console.WriteLine($"[WEBHOOK] Event received. Payload: {payload.Length} chars");

        if (string.IsNullOrEmpty(signature))
        {
            Console.WriteLine("[WEBHOOK] Missing signature header");
            return BadRequest(new { message = "Missing Stripe-Signature." });
        }

        try
        {
            var isValid = _payment.ValidateWebhookSignature(
                payload, signature,
                out var eventType,
                out var paymentIntentId);

            Console.WriteLine($"[WEBHOOK] Valid: {isValid}, Type: {eventType}, PI: {paymentIntentId}");

            if (!isValid)
                return BadRequest(new { message = "Invalid webhook signature." });

            if (eventType == "payment_intent.succeeded" &&
                !string.IsNullOrEmpty(paymentIntentId))
            {
                await _sender.Send(
                    new HandlePaymentSuccessCommand(paymentIntentId), ct);

                Console.WriteLine($"[WEBHOOK] Order completed for PI: {paymentIntentId}");
            }

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WEBHOOK] Exception: {ex.Message}");
            Console.WriteLine(ex.ToString());
            throw;  // يرجع الـ ExceptionMiddleware يتعامل معاه
        }
    }
}
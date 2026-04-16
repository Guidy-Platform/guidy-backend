using MediatR;

namespace CoursePlatform.Application.Features.Orders.Commands.HandlePaymentSuccess;

public record HandlePaymentSuccessCommand(
    string PaymentIntentId
) : IRequest<Unit>;
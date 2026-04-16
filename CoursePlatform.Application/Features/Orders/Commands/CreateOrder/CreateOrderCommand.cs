using CoursePlatform.Application.Features.Orders.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Orders.Commands.CreateOrder;

public record CreateOrderCommand(
    List<int> CourseIds
) : IRequest<CreateOrderResultDto>;
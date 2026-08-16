using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using MediatR;

namespace HomeTaste.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public record UpdateOrderStatusCommand(Guid Id, OrderStatus Status, string? CancellationReason)
        : IRequest<Result<OrderResponse>>;
}

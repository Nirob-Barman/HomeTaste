using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Orders.Commands.PlaceOrder
{
    public record PlaceOrderCommand(
        Guid AddressId,
        List<PlaceOrderItemCommand> Items,
        string? CouponCode,
        int PointsToRedeem,
        string? Notes) : IRequest<Result<OrderResponse>>;

    public record PlaceOrderItemCommand(
        Guid MealId,
        int Quantity,
        string? SpecialInstructions,
        List<Guid>? CustomizationOptionIds);
}

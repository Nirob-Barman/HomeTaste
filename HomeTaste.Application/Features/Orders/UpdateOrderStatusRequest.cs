using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Features.Orders
{
    public record UpdateOrderStatusRequest
    {
        public OrderStatus Status { get; set; }
        public string? CancellationReason { get; set; }
    }
}

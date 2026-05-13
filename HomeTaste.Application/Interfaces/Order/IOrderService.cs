using HomeTaste.Application.DTOs.Order;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Interfaces.Order
{
    public interface IOrderService
    {
        Task<Result<PaginatedResponse<IEnumerable<OrderResponse>>>> GetMyOrdersAsync(int pageNumber = 1, int pageSize = 10);
        Task<Result<OrderResponse>> GetOrderByIdAsync(Guid id);
        Task<Result<PaginatedResponse<IEnumerable<OrderResponse>>>> GetAllOrdersAsync(int pageNumber = 1, int pageSize = 10, OrderStatus? status = null);
        Task<Result<OrderResponse>> PlaceOrderAsync(CreateOrderRequest request);
        Task<Result<OrderResponse>> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusRequest request);
        Task<Result<bool>> CancelOrderAsync(Guid id, CancelOrderRequest request);
    }
}

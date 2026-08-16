using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using MediatR;

namespace HomeTaste.Application.Features.Orders.Queries.GetAllOrders
{
    public record GetAllOrdersQuery(int PageNumber = 1, int PageSize = 10, OrderStatus? Status = null)
        : IRequest<Result<PaginatedResponse<IEnumerable<OrderResponse>>>>;
}

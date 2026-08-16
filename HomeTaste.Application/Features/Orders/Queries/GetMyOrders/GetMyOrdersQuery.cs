using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Orders.Queries.GetMyOrders
{
    public record GetMyOrdersQuery(int PageNumber = 1, int PageSize = 10)
        : IRequest<Result<PaginatedResponse<IEnumerable<OrderResponse>>>>;
}

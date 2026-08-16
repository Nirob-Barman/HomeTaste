using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Orders.Queries.GetOrderById
{
    public record GetOrderByIdQuery(Guid Id) : IRequest<Result<OrderResponse>>;
}

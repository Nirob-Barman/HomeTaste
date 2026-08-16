using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Orders.Commands.CancelOrder
{
    public record CancelOrderCommand(Guid Id, string? Reason) : IRequest<Result<bool>>;
}

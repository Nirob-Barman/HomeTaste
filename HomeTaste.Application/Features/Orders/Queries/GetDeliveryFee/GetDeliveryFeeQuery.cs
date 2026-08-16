using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Orders.Queries.GetDeliveryFee
{
    public record GetDeliveryFeeQuery(decimal SubTotal) : IRequest<Result<DeliveryFeeResponse>>;
}

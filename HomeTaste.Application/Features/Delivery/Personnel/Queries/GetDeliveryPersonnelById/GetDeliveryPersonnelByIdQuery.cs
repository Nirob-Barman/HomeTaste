using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Personnel.Queries.GetDeliveryPersonnelById
{
    public record GetDeliveryPersonnelByIdQuery(Guid Id) : IRequest<Result<DeliveryPersonnelResponse>>;
}

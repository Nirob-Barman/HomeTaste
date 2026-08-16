using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.DeliveryZones.Queries.CheckServiceability
{
    public record CheckServiceabilityQuery(Guid AddressId) : IRequest<Result<ServiceabilityResponse>>;
}

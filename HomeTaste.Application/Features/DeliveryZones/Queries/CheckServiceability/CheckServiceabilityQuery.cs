using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.DeliveryZones.Queries.CheckServiceability
{
    public class CheckServiceabilityQuery : IRequest<Result<ServiceabilityResponse>>
    {
        public Guid AddressId { get; set; }

        public CheckServiceabilityQuery(Guid addressId)
        {
            AddressId = addressId;
        }
    }
}

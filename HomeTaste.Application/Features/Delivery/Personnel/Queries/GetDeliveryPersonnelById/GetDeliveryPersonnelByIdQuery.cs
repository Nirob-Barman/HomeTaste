using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Personnel.Queries.GetDeliveryPersonnelById
{
    public class GetDeliveryPersonnelByIdQuery : IRequest<Result<DeliveryPersonnelResponse>>
    {
        public Guid Id { get; set; }

        public GetDeliveryPersonnelByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}

using HomeTaste.Application.Features.Delivery;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Personnel.Queries.GetAllDeliveryPersonnel
{
    public class GetAllDeliveryPersonnelQuery : IRequest<Result<PaginatedResponse<IEnumerable<DeliveryPersonnelResponse>>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

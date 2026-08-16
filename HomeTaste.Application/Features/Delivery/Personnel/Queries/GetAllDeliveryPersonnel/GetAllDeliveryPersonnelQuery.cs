using HomeTaste.Application.Features.Delivery;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Personnel.Queries.GetAllDeliveryPersonnel
{
    public record GetAllDeliveryPersonnelQuery(int PageNumber = 1, int PageSize = 10)
        : IRequest<Result<PaginatedResponse<IEnumerable<DeliveryPersonnelResponse>>>>;
}

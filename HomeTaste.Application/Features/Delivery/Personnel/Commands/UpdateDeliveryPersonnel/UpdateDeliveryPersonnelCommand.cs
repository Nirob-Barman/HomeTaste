using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Personnel.Commands.UpdateDeliveryPersonnel
{
    public record UpdateDeliveryPersonnelCommand(
        Guid Id,
        string? FullName,
        string? Phone,
        string? VehicleType,
        string? VehicleNumber) : IRequest<Result<DeliveryPersonnelResponse>>;
}

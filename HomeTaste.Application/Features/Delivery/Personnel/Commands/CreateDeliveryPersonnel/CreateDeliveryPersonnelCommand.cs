using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Personnel.Commands.CreateDeliveryPersonnel
{
    public record CreateDeliveryPersonnelCommand(
        string? UserId,
        string? FullName,
        string? Phone,
        string? VehicleType,
        string? VehicleNumber) : IRequest<Result<DeliveryPersonnelResponse>>;
}

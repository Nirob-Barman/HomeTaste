using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Personnel.Commands.UpdateLocation
{
    public record UpdateLocationCommand(Guid Id, double Latitude, double Longitude) : IRequest<Result<bool>>;
}

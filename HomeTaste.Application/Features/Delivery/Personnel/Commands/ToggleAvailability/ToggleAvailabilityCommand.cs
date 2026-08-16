using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Personnel.Commands.ToggleAvailability
{
    public class ToggleAvailabilityCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public ToggleAvailabilityCommand(Guid id)
        {
            Id = id;
        }
    }
}

using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCustomizations.Commands.ToggleAvailability
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

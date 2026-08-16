using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCustomizations.Commands.ToggleAvailability
{
    public record ToggleAvailabilityCommand(Guid Id) : IRequest<Result<bool>>;
}

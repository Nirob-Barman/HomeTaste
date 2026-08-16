using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCustomizations.Commands.DeleteOption
{
    public record DeleteOptionCommand(Guid Id) : IRequest<Result<bool>>;
}

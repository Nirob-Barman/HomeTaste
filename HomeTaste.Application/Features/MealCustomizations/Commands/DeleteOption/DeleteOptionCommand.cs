using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCustomizations.Commands.DeleteOption
{
    public class DeleteOptionCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public DeleteOptionCommand(Guid id)
        {
            Id = id;
        }
    }
}

using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCustomizations.Commands.UpdateOption
{
    public class UpdateOptionCommand : IRequest<Result<MealCustomizationOptionResponse>>
    {
        public Guid Id { get; set; }
        public MealCustomizationOptionRequest Request { get; set; }

        public UpdateOptionCommand(Guid id, MealCustomizationOptionRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}

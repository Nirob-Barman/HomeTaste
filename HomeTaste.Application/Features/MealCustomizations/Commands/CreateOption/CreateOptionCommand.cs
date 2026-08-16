using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCustomizations.Commands.CreateOption
{
    public class CreateOptionCommand : IRequest<Result<MealCustomizationOptionResponse>>
    {
        public MealCustomizationOptionRequest Request { get; set; }

        public CreateOptionCommand(MealCustomizationOptionRequest request)
        {
            Request = request;
        }
    }
}

using HomeTaste.Application.DTOs.File;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Ingredients.Commands.CreateIngredient
{
    public class CreateIngredientCommand : IRequest<Result<IngredientResponse>>
    {
        public IngredientRequest IngredientRequest { get; set; }
        public FileUploadDto? File { get; set; }

        public CreateIngredientCommand(IngredientRequest ingredientRequest, FileUploadDto? file)
        {
            IngredientRequest = ingredientRequest;
            File = file;
        }
    }
}

using HomeTaste.Application.DTOs.Support;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.CategoryTypes.Commands.CreateCategoryType
{
    public class CreateCategoryTypeCommand : IRequest<Result<CategoryTypeResponse>>
    {
        public CategoryTypeRequest CategoryTypeRequest { get; set; }

        public CreateCategoryTypeCommand(CategoryTypeRequest categoryTypeRequest)
        {
            CategoryTypeRequest = categoryTypeRequest;
        }
    }
}

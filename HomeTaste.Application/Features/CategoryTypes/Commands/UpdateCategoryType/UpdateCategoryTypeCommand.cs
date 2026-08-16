using HomeTaste.Application.DTOs.Support;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.CategoryTypes.Commands.UpdateCategoryType
{
    public class UpdateCategoryTypeCommand : IRequest<Result<CategoryTypeResponse>>
    {
        public Guid Id { get; set; }
        public CategoryTypeRequest CategoryTypeRequest { get; set; }

        public UpdateCategoryTypeCommand(Guid id, CategoryTypeRequest categoryTypeRequest)
        {
            Id = id;
            CategoryTypeRequest = categoryTypeRequest;
        }
    }
}

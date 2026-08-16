using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.CategoryTypes.Commands.UpdateCategoryType
{
    public record UpdateCategoryTypeCommand(Guid Id, string? Name, string? Description)
        : IRequest<Result<CategoryTypeResponse>>;
}

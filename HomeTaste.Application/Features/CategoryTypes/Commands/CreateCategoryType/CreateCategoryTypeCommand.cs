using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.CategoryTypes.Commands.CreateCategoryType
{
    public record CreateCategoryTypeCommand(string? Name, string? Description)
        : IRequest<Result<CategoryTypeResponse>>;
}

using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.CategoryTypes.Commands.DeleteCategoryType
{
    public record DeleteCategoryTypeCommand(Guid Id) : IRequest<Result<bool>>;
}

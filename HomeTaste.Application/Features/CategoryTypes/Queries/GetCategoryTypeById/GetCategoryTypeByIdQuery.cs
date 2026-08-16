using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.CategoryTypes.Queries.GetCategoryTypeById
{
    public record GetCategoryTypeByIdQuery(Guid Id) : IRequest<Result<CategoryTypeResponse>>;
}

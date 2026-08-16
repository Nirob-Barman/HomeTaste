using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.CategoryTypes.Queries.GetCategoryTypeById
{
    public class GetCategoryTypeByIdQuery : IRequest<Result<CategoryTypeResponse>>
    {
        public Guid Id { get; set; }

        public GetCategoryTypeByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}

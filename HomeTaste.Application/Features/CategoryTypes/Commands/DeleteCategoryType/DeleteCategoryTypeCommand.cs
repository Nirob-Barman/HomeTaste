using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.CategoryTypes.Commands.DeleteCategoryType
{
    public class DeleteCategoryTypeCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public DeleteCategoryTypeCommand(Guid id)
        {
            Id = id;
        }
    }
}

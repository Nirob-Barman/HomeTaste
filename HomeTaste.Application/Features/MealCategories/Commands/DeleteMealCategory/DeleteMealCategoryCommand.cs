using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCategories.Commands.DeleteMealCategory
{
    public class DeleteMealCategoryCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public DeleteMealCategoryCommand(Guid id)
        {
            Id = id;
        }
    }
}

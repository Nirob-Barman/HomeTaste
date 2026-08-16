using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCategories.Commands.UpdateMealCategory
{
    public class UpdateMealCategoryCommand : IRequest<Result<MealCategoryResponse>>
    {
        public Guid Id { get; set; }
        public MealCategoryRequest MealCategoryRequest { get; set; }

        public UpdateMealCategoryCommand(Guid id, MealCategoryRequest mealCategoryRequest)
        {
            Id = id;
            MealCategoryRequest = mealCategoryRequest;
        }
    }
}

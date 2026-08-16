using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCategories.Commands.CreateMealCategory
{
    public class CreateMealCategoryCommand : IRequest<Result<MealCategoryResponse>>
    {
        public MealCategoryRequest MealCategoryRequest { get; set; }

        public CreateMealCategoryCommand(MealCategoryRequest mealCategoryRequest)
        {
            MealCategoryRequest = mealCategoryRequest;
        }
    }
}

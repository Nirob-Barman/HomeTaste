using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.MealManagement;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.MealCategories.Commands.CreateMealCategory
{
    public class CreateMealCategoryCommandHandler : IRequestHandler<CreateMealCategoryCommand, Result<MealCategoryResponse>>
    {
        private readonly IApplicationDbContext _context;

        public CreateMealCategoryCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<MealCategoryResponse>> Handle(CreateMealCategoryCommand request, CancellationToken cancellationToken)
        {
            var mealCategoryRequest = request.MealCategoryRequest;

            var existingCategory = await _context.MealCategories.FirstOrDefaultAsync(c => c.Name == mealCategoryRequest.Name, cancellationToken);

            if (existingCategory != null)
            {
                return Result<MealCategoryResponse>.Fail("Meal category with the same name already exists.", "Duplicate meal category", ResultType.Conflict);
            }

            var mealCategory = MealCategory.Create(mealCategoryRequest.Name, mealCategoryRequest.Description);

            _context.MealCategories.Add(mealCategory);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new MealCategoryResponse
            {
                Id = mealCategory.Id,
                Name = mealCategory.Name,
                Description = mealCategory.Description,
                ImageUrl = mealCategory.ImageUrl
            };

            return Result<MealCategoryResponse>.Ok(response, "Meal category created successfully", ResultType.Success);
        }
    }
}

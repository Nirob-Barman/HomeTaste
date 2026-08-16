using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.MealCategories.Commands.UpdateMealCategory
{
    public class UpdateMealCategoryCommandHandler : IRequestHandler<UpdateMealCategoryCommand, Result<MealCategoryResponse>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateMealCategoryCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<MealCategoryResponse>> Handle(UpdateMealCategoryCommand request, CancellationToken cancellationToken)
        {
            var id = request.Id;
            var mealCategoryRequest = request.MealCategoryRequest;

            var mealCategory = await _context.MealCategories.FindAsync(new object?[] { id }, cancellationToken);
            if (mealCategory == null)
                return Result<MealCategoryResponse>.Fail("Meal category not found", "Meal category not found", ResultType.NotFound);

            var existingCategory = await _context.MealCategories
                .FirstOrDefaultAsync(c => c.Name == mealCategoryRequest.Name && c.Id != id, cancellationToken);

            if (existingCategory != null)
            {
                return Result<MealCategoryResponse>.Fail("Meal category with the same name already exists.", "Duplicate meal category", ResultType.Conflict);
            }

            mealCategory.UpdateDetails(mealCategoryRequest.Name, mealCategoryRequest.Description);

            await _context.SaveChangesAsync(cancellationToken);

            var response = new MealCategoryResponse
            {
                Id = mealCategory.Id,
                Name = mealCategory.Name,
                Description = mealCategory.Description,
                ImageUrl = mealCategory.ImageUrl
            };

            return Result<MealCategoryResponse>.Ok(response, "Meal category updated successfully", ResultType.Success);
        }
    }
}

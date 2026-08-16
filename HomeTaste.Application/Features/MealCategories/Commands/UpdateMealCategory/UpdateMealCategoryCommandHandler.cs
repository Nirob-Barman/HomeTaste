using HomeTaste.Application.Common.Exceptions;
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

        public async Task<Result<MealCategoryResponse>> Handle(UpdateMealCategoryCommand command, CancellationToken cancellationToken)
        {
            var mealCategory = await _context.MealCategories.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (mealCategory == null)
                throw new NotFoundException("Meal category not found");

            var existingCategory = await _context.MealCategories
                .FirstOrDefaultAsync(c => c.Name == command.Name && c.Id != command.Id, cancellationToken);

            if (existingCategory != null)
            {
                throw new ConflictException("Meal category with the same name already exists.");
            }

            mealCategory.UpdateDetails(command.Name, command.Description);

            await _context.SaveChangesAsync(cancellationToken);

            var response = new MealCategoryResponse
            {
                Id = mealCategory.Id,
                Name = mealCategory.Name,
                Description = mealCategory.Description,
                ImageUrl = mealCategory.ImageUrl
            };

            return Result<MealCategoryResponse>.Ok(response, "Meal category updated successfully");
        }
    }
}

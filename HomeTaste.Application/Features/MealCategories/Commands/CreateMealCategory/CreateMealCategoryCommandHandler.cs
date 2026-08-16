using HomeTaste.Application.Common.Exceptions;
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

        public async Task<Result<MealCategoryResponse>> Handle(CreateMealCategoryCommand command, CancellationToken cancellationToken)
        {
            var existingCategory = await _context.MealCategories.FirstOrDefaultAsync(c => c.Name == command.Name, cancellationToken);

            if (existingCategory != null)
            {
                throw new ConflictException("Meal category with the same name already exists.");
            }

            var mealCategory = MealCategory.Create(command.Name, command.Description);

            _context.MealCategories.Add(mealCategory);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new MealCategoryResponse
            {
                Id = mealCategory.Id,
                Name = mealCategory.Name,
                Description = mealCategory.Description,
                ImageUrl = mealCategory.ImageUrl
            };

            return Result<MealCategoryResponse>.Ok(response, "Meal category created successfully");
        }
    }
}

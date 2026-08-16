using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCategories.Commands.DeleteMealCategory
{
    public class DeleteMealCategoryCommandHandler : IRequestHandler<DeleteMealCategoryCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteMealCategoryCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteMealCategoryCommand command, CancellationToken cancellationToken)
        {
            var mealCategory = await _context.MealCategories.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (mealCategory == null)
                throw new NotFoundException("Meal category not found");

            _context.MealCategories.Remove(mealCategory);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<bool>.Ok(true, "Meal category deleted successfully");
        }
    }
}

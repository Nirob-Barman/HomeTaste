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

        public async Task<Result<bool>> Handle(DeleteMealCategoryCommand request, CancellationToken cancellationToken)
        {
            var mealCategory = await _context.MealCategories.FindAsync(new object?[] { request.Id }, cancellationToken);
            if (mealCategory == null)
                return Result<bool>.Fail("Meal category not found", "Meal category not found", ResultType.NotFound);

            _context.MealCategories.Remove(mealCategory);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<bool>.Ok(true, "Meal category deleted successfully", ResultType.Success);
        }
    }
}

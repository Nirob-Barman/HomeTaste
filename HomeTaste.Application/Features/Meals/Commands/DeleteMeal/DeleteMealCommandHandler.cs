using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Meals.Commands.DeleteMeal
{
    public class DeleteMealCommandHandler : IRequestHandler<DeleteMealCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteMealCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteMealCommand command, CancellationToken cancellationToken)
        {
            var meal = await _context.Meals.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (meal == null)
                throw new NotFoundException("Meal not found");

            _context.Meals.Remove(meal);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, "Meal deleted successfully");
        }
    }
}

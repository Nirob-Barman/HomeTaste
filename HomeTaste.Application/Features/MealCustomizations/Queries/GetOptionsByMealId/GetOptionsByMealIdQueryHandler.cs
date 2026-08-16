using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.MealCustomizations.Queries.GetOptionsByMealId
{
    public class GetOptionsByMealIdQueryHandler : IRequestHandler<GetOptionsByMealIdQuery, Result<IEnumerable<MealCustomizationOptionResponse>>>
    {
        private readonly IApplicationDbContext _context;

        public GetOptionsByMealIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<MealCustomizationOptionResponse>>> Handle(GetOptionsByMealIdQuery request, CancellationToken cancellationToken)
        {
            var meal = await _context.Meals.FindAsync(new object?[] { request.MealId }, cancellationToken);
            if (meal == null)
                throw new NotFoundException("Meal not found.");

            var options = await _context.MealCustomizationOptions
                .Where(o => o.MealId == request.MealId)
                .ToListAsync(cancellationToken);

            var response = options.Select(o => MealCustomizationOptionMapper.ToResponse(o, meal.Name));
            return Result<IEnumerable<MealCustomizationOptionResponse>>.Ok(response, "Options retrieved successfully");
        }
    }
}

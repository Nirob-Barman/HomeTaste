using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Meals.Queries.GetMealById
{
    public class GetMealByIdQueryHandler : IRequestHandler<GetMealByIdQuery, Result<MealResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetMealByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<MealResponse>> Handle(GetMealByIdQuery request, CancellationToken cancellationToken)
        {
            var response = await _context.Meals
                .Where(m => m.Id == request.Id)
                .Select(m => new MealResponse(
                    m.Id,
                    m.Name,
                    m.Description,
                    m.Price,
                    m.ImageUrl,
                    m.CategoryId,
                    m.IsAvailable,
                    m.PreparationTime,
                    m.DiscountPrice,
                    m.Calories))
                .FirstOrDefaultAsync(cancellationToken);

            if (response == null)
                throw new NotFoundException("Meal not found");

            return Result<MealResponse>.Ok(response, "Meal retrieved successfully");
        }
    }
}

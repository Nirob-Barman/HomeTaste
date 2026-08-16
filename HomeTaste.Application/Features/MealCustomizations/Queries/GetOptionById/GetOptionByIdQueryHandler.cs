using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCustomizations.Queries.GetOptionById
{
    public class GetOptionByIdQueryHandler : IRequestHandler<GetOptionByIdQuery, Result<MealCustomizationOptionResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetOptionByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<MealCustomizationOptionResponse>> Handle(GetOptionByIdQuery request, CancellationToken cancellationToken)
        {
            var option = await _context.MealCustomizationOptions.FindAsync(new object?[] { request.Id }, cancellationToken);
            if (option == null)
                throw new NotFoundException("Option not found.");

            var meal = await _context.Meals.FindAsync(new object?[] { option.MealId }, cancellationToken);

            return Result<MealCustomizationOptionResponse>.Ok(MealCustomizationOptionMapper.ToResponse(option, meal?.Name), "Option retrieved successfully");
        }
    }
}

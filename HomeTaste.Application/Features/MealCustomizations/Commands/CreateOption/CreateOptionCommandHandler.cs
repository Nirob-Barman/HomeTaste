using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.MealManagement;
using MediatR;

namespace HomeTaste.Application.Features.MealCustomizations.Commands.CreateOption
{
    public class CreateOptionCommandHandler : IRequestHandler<CreateOptionCommand, Result<MealCustomizationOptionResponse>>
    {
        private readonly IApplicationDbContext _context;

        public CreateOptionCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<MealCustomizationOptionResponse>> Handle(CreateOptionCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var meal = await _context.Meals.FindAsync(new object?[] { request.MealId }, cancellationToken);
            if (meal == null)
                throw new NotFoundException("Meal not found.");

            var option = MealCustomizationOption.Create(request.MealId, request.Name, request.AdditionalPrice, request.IsAvailable, request.OptionType);

            _context.MealCustomizationOptions.Add(option);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<MealCustomizationOptionResponse>.Ok(MealCustomizationOptionMapper.ToResponse(option, meal.Name), "Option created successfully");
        }
    }
}

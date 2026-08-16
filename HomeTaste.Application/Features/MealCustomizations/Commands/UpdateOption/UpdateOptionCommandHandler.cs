using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCustomizations.Commands.UpdateOption
{
    public class UpdateOptionCommandHandler : IRequestHandler<UpdateOptionCommand, Result<MealCustomizationOptionResponse>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateOptionCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<MealCustomizationOptionResponse>> Handle(UpdateOptionCommand command, CancellationToken cancellationToken)
        {
            var option = await _context.MealCustomizationOptions.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (option == null)
                throw new NotFoundException("Option not found.");

            var meal = await _context.Meals.FindAsync(new object?[] { command.MealId }, cancellationToken);
            if (meal == null)
                throw new NotFoundException("Meal not found.");

            option.UpdateDetails(command.MealId, command.Name, command.AdditionalPrice, command.IsAvailable, command.OptionType);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<MealCustomizationOptionResponse>.Ok(MealCustomizationOptionMapper.ToResponse(option, meal.Name), "Option updated successfully");
        }
    }
}

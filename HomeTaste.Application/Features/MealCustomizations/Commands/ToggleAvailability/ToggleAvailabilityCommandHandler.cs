using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCustomizations.Commands.ToggleAvailability
{
    public class ToggleAvailabilityCommandHandler : IRequestHandler<ToggleAvailabilityCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public ToggleAvailabilityCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(ToggleAvailabilityCommand command, CancellationToken cancellationToken)
        {
            var option = await _context.MealCustomizationOptions.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (option == null)
                throw new NotFoundException("Option not found.");

            option.ToggleAvailability();

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(option.IsAvailable, $"Option marked as {(option.IsAvailable ? "available" : "unavailable")}");
        }
    }
}

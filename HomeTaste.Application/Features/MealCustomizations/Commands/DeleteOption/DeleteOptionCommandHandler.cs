using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCustomizations.Commands.DeleteOption
{
    public class DeleteOptionCommandHandler : IRequestHandler<DeleteOptionCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteOptionCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteOptionCommand command, CancellationToken cancellationToken)
        {
            var option = await _context.MealCustomizationOptions.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (option == null)
                throw new NotFoundException("Option not found.");

            _context.MealCustomizationOptions.Remove(option);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, "Option deleted successfully");
        }
    }
}

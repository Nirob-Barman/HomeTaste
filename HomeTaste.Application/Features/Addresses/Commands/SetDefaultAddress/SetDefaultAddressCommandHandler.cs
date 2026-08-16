using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Addresses.Commands.SetDefaultAddress
{
    public class SetDefaultAddressCommandHandler : IRequestHandler<SetDefaultAddressCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public SetDefaultAddressCommandHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<bool>> Handle(SetDefaultAddressCommand command, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                throw new UnauthorizedException("Invalid user.");

            var address = await _context.Addresses.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (address == null)
                throw new NotFoundException("Address not found.");

            if (address.UserId != userId)
                throw new ForbiddenAccessException("Access denied.");

            await AddressDefaultHelper.ClearDefaultFlagAsync(_context, userId, cancellationToken);

            address.SetAsDefault();

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, "Default address updated successfully");
        }
    }
}

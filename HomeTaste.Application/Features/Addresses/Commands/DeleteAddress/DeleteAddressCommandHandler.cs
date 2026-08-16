using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Addresses.Commands.DeleteAddress
{
    public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public DeleteAddressCommandHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<bool>> Handle(DeleteAddressCommand command, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                throw new UnauthorizedException("Invalid user.");

            var address = await _context.Addresses.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (address == null)
                throw new NotFoundException("Address not found.");

            if (address.UserId != userId && !_userContextService.IsInRole("Admin"))
                throw new ForbiddenAccessException("Access denied.");

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, "Address deleted successfully");
        }
    }
}

using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Addresses.Commands.UpdateAddress
{
    public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, Result<AddressResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public UpdateAddressCommandHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<AddressResponse>> Handle(UpdateAddressCommand command, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                throw new UnauthorizedException("Invalid user.");

            var address = await _context.Addresses.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (address == null)
                throw new NotFoundException("Address not found.");

            if (address.UserId != userId)
                throw new ForbiddenAccessException("Access denied.");

            if (command.IsDefault && !address.IsDefault)
                await AddressDefaultHelper.ClearDefaultFlagAsync(_context, userId, cancellationToken);

            address.UpdateDetails(
                command.Label,
                command.AddressLine1,
                command.AddressLine2,
                command.City,
                command.State,
                command.PostalCode,
                command.Country,
                command.Latitude,
                command.Longitude,
                command.IsDefault);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<AddressResponse>.Ok(AddressMapper.ToResponse(address), "Address updated successfully");
        }
    }
}

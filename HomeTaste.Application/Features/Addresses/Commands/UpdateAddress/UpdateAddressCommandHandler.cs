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
            var request = command.Request;

            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                throw new UnauthorizedException("Invalid user.");

            var address = await _context.Addresses.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (address == null)
                throw new NotFoundException("Address not found.");

            if (address.UserId != userId)
                throw new ForbiddenAccessException("Access denied.");

            if (request.IsDefault && !address.IsDefault)
                await AddressDefaultHelper.ClearDefaultFlagAsync(_context, userId, cancellationToken);

            address.UpdateDetails(
                request.Label,
                request.AddressLine1,
                request.AddressLine2,
                request.City,
                request.State,
                request.PostalCode,
                request.Country,
                request.Latitude,
                request.Longitude,
                request.IsDefault);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<AddressResponse>.Ok(AddressMapper.ToResponse(address), "Address updated successfully");
        }
    }
}

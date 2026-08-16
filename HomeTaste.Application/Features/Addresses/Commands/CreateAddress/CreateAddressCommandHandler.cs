using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using AddressEntity = HomeTaste.Domain.Entities.Address.Address;

namespace HomeTaste.Application.Features.Addresses.Commands.CreateAddress
{
    public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, Result<AddressResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public CreateAddressCommandHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<AddressResponse>> Handle(CreateAddressCommand command, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                throw new UnauthorizedException("Invalid user.");

            if (command.IsDefault)
                await AddressDefaultHelper.ClearDefaultFlagAsync(_context, userId, cancellationToken);

            var address = AddressEntity.Create(
                userId,
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

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<AddressResponse>.Ok(AddressMapper.ToResponse(address), "Address created successfully");
        }
    }
}

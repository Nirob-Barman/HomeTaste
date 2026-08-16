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
            var request = command.Request;

            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                throw new UnauthorizedException("Invalid user.");

            if (request.IsDefault)
                await AddressDefaultHelper.ClearDefaultFlagAsync(_context, userId, cancellationToken);

            var address = AddressEntity.Create(
                userId,
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

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<AddressResponse>.Ok(AddressMapper.ToResponse(address), "Address created successfully");
        }
    }
}

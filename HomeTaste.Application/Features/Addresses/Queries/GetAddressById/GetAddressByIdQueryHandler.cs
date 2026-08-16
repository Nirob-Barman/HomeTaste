using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Addresses.Queries.GetAddressById
{
    public class GetAddressByIdQueryHandler : IRequestHandler<GetAddressByIdQuery, Result<AddressResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetAddressByIdQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<AddressResponse>> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                throw new UnauthorizedException("Invalid user.");

            var address = await _context.Addresses.FindAsync(new object?[] { request.Id }, cancellationToken);
            if (address == null)
                throw new NotFoundException("Address not found.");

            if (address.UserId != userId && !_userContextService.IsInRole("Admin"))
                throw new ForbiddenAccessException("Access denied.");

            return Result<AddressResponse>.Ok(AddressMapper.ToResponse(address), "Address retrieved successfully");
        }
    }
}

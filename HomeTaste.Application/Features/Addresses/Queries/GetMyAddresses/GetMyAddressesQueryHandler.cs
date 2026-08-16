using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Addresses.Queries.GetMyAddresses
{
    public class GetMyAddressesQueryHandler : IRequestHandler<GetMyAddressesQuery, Result<IEnumerable<AddressResponse>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetMyAddressesQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<IEnumerable<AddressResponse>>> Handle(GetMyAddressesQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                throw new UnauthorizedException("Invalid user.");

            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync(cancellationToken);

            var response = addresses.Select(AddressMapper.ToResponse);
            return Result<IEnumerable<AddressResponse>>.Ok(response, "Addresses retrieved successfully");
        }
    }
}

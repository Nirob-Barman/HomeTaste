using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Loyalty.Queries.GetMyAccount
{
    public class GetMyAccountQueryHandler : IRequestHandler<GetMyAccountQuery, Result<LoyaltyAccountResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetMyAccountQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<LoyaltyAccountResponse>> Handle(GetMyAccountQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("Invalid user.");

            var account = await LoyaltyAccountHelper.GetOrCreateAccountAsync(_context, userId, cancellationToken);
            return Result<LoyaltyAccountResponse>.Ok(LoyaltyAccountHelper.ToResponse(account), "Loyalty account retrieved.");
        }
    }
}

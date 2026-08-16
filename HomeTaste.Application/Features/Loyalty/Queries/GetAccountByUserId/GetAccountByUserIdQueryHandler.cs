using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Loyalty.Queries.GetAccountByUserId
{
    public class GetAccountByUserIdQueryHandler : IRequestHandler<GetAccountByUserIdQuery, Result<LoyaltyAccountResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetAccountByUserIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<LoyaltyAccountResponse>> Handle(GetAccountByUserIdQuery request, CancellationToken cancellationToken)
        {
            var account = await _context.LoyaltyAccounts.FirstOrDefaultAsync(a => a.UserId == request.UserId, cancellationToken);
            if (account == null)
                throw new NotFoundException("No loyalty account found for this user.");

            return Result<LoyaltyAccountResponse>.Ok(LoyaltyAccountHelper.ToResponse(account), "Account retrieved.");
        }
    }
}

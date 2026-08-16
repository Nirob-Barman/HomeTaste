using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Loyalty.Queries.GetMyTransactions
{
    public class GetMyTransactionsQueryHandler : IRequestHandler<GetMyTransactionsQuery, Result<PaginatedResponse<IEnumerable<LoyaltyTransactionResponse>>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetMyTransactionsQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<LoyaltyTransactionResponse>>>> Handle(GetMyTransactionsQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("Invalid user.");

            var account = await _context.LoyaltyAccounts.FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);

            if (account == null)
                return Result<PaginatedResponse<IEnumerable<LoyaltyTransactionResponse>>>.Ok(
                    new PaginatedResponse<IEnumerable<LoyaltyTransactionResponse>>
                    {
                        Data = Enumerable.Empty<LoyaltyTransactionResponse>(),
                        MetaData = PaginationHelper.GetPaginationMetadata(request.PageNumber, request.PageSize, 0)
                    }, "No transactions found.");

            var query = _context.LoyaltyTransactions
                .Where(t => t.LoyaltyAccountId == account.Id)
                .OrderByDescending(t => t.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);

            var page = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var transactions = page
                .Select(t => new LoyaltyTransactionResponse(
                    t.Id,
                    t.Points,
                    t.TransactionType,
                    t.TransactionType.ToString(),
                    t.ReferenceId,
                    t.Description,
                    t.CreatedAt))
                .ToList();

            var meta = PaginationHelper.GetPaginationMetadata(request.PageNumber, request.PageSize, totalCount);
            return Result<PaginatedResponse<IEnumerable<LoyaltyTransactionResponse>>>.Ok(
                new PaginatedResponse<IEnumerable<LoyaltyTransactionResponse>> { Data = transactions, MetaData = meta },
                "Transactions retrieved.");
        }
    }
}

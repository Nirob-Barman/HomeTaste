using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Loyalty.Queries.GetMyTransactions
{
    public record GetMyTransactionsQuery(int PageNumber = 1, int PageSize = 20)
        : IRequest<Result<PaginatedResponse<IEnumerable<LoyaltyTransactionResponse>>>>;
}

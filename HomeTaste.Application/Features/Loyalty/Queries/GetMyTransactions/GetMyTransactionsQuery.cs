using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Loyalty.Queries.GetMyTransactions
{
    public class GetMyTransactionsQuery : IRequest<Result<PaginatedResponse<IEnumerable<LoyaltyTransactionResponse>>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}

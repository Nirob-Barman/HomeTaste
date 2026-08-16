using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Payments.Queries.GetAllPayments
{
    public class GetAllPaymentsQueryHandler : IRequestHandler<GetAllPaymentsQuery, Result<PaginatedResponse<IEnumerable<PaymentTransactionResponse>>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllPaymentsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<PaymentTransactionResponse>>>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.PaymentTransactions.AsQueryable();

            if (request.Status.HasValue)
                query = query.Where(t => t.Status == request.Status.Value);

            query = query.OrderByDescending(t => t.UpdatedAt ?? t.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);

            var transactions = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var response = transactions.Select(PaymentMapper.ToResponse).ToList();
            var meta = PaginationHelper.GetPaginationMetadata(request.PageNumber, request.PageSize, totalCount);

            return Result<PaginatedResponse<IEnumerable<PaymentTransactionResponse>>>.Ok(
                new PaginatedResponse<IEnumerable<PaymentTransactionResponse>> { Data = response, MetaData = meta },
                "Transactions retrieved successfully.");
        }
    }
}

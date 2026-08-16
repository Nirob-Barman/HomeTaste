using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, Result<PaginatedResponse<IEnumerable<OrderResponse>>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllOrdersQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<OrderResponse>>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Orders
                .Include(o => o.Address)
                .Include(o => o.Coupon)
                .Include(o => o.OrderItems)
                .AsQueryable();

            if (request.Status.HasValue)
                query = query.Where(o => o.Status == request.Status.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var orders = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var response = await OrderMapper.BuildOrderResponsesAsync(_context, orders, cancellationToken);
            var meta = PaginationHelper.GetPaginationMetadata(request.PageNumber, request.PageSize, totalCount);

            return Result<PaginatedResponse<IEnumerable<OrderResponse>>>.Ok(
                new PaginatedResponse<IEnumerable<OrderResponse>> { Data = response, MetaData = meta },
                "Orders retrieved successfully.");
        }
    }
}

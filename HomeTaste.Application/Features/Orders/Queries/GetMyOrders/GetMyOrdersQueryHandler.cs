using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Orders.Queries.GetMyOrders
{
    public class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, Result<PaginatedResponse<IEnumerable<OrderResponse>>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetMyOrdersQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<OrderResponse>>>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                throw new UnauthorizedException("Invalid user.");

            var query = _context.Orders
                .Include(o => o.Address)
                .Include(o => o.Coupon)
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId);

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

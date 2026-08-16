using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetOrderByIdQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                throw new UnauthorizedException("Invalid user.");

            var order = await _context.Orders.FindAsync(new object?[] { request.Id }, cancellationToken);
            if (order == null)
                throw new NotFoundException("Order not found.");

            if (order.UserId != userId && !_userContextService.IsInRole("Admin"))
                throw new ForbiddenAccessException("Access denied.");

            var response = await OrderMapper.BuildOrderResponseAsync(_context, order, cancellationToken);
            return Result<OrderResponse>.Ok(response, "Order retrieved successfully.");
        }
    }
}

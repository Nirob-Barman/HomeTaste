using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Analytics.Queries.GetTopCustomers
{
    public class GetTopCustomersQueryHandler : IRequestHandler<GetTopCustomersQuery, Result<List<TopCustomerItem>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserManager _userManager;

        public GetTopCustomersQueryHandler(IApplicationDbContext context, IUserManager userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Result<List<TopCustomerItem>>> Handle(GetTopCustomersQuery request, CancellationToken cancellationToken)
        {
            var result = await AnalyticsCalculations.GetTopCustomersAsync(_context, _userManager, request.Top, cancellationToken);
            return Result<List<TopCustomerItem>>.Ok(result, "Top customers retrieved successfully");
        }
    }
}

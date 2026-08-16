using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Analytics.Queries.GetTopCustomers
{
    public class GetTopCustomersQuery : IRequest<Result<List<TopCustomerItem>>>
    {
        public int Top { get; set; } = 10;
    }
}

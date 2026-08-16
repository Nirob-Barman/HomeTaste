using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Analytics.Queries.GetTopCustomers
{
    public record GetTopCustomersQuery(int Top = 10) : IRequest<Result<List<TopCustomerItem>>>;
}

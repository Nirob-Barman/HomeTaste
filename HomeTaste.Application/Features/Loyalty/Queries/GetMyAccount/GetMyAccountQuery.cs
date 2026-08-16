using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Loyalty.Queries.GetMyAccount
{
    public class GetMyAccountQuery : IRequest<Result<LoyaltyAccountResponse>>
    {
    }
}

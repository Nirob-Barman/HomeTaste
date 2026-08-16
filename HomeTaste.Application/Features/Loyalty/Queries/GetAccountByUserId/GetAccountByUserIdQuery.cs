using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Loyalty.Queries.GetAccountByUserId
{
    public class GetAccountByUserIdQuery : IRequest<Result<LoyaltyAccountResponse>>
    {
        public string UserId { get; set; }

        public GetAccountByUserIdQuery(string userId)
        {
            UserId = userId;
        }
    }
}

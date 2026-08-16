using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Loyalty.Queries.GetAccountByUserId
{
    public record GetAccountByUserIdQuery(string UserId) : IRequest<Result<LoyaltyAccountResponse>>;
}

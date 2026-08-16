using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Loyalty.Queries.GetMyAccount
{
    public record GetMyAccountQuery : IRequest<Result<LoyaltyAccountResponse>>;
}

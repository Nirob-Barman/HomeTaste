using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Coupons.Queries.GetAllCoupons
{
    public record GetAllCouponsQuery(int PageNumber = 1, int PageSize = 10, string? SearchTerm = null)
        : IRequest<Result<PaginatedResponse<IEnumerable<CouponResponse>>>>;
}

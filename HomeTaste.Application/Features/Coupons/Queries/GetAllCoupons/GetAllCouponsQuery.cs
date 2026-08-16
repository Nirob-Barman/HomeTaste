using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Coupons.Queries.GetAllCoupons
{
    public class GetAllCouponsQuery : IRequest<Result<PaginatedResponse<IEnumerable<CouponResponse>>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; } = null!;
    }
}

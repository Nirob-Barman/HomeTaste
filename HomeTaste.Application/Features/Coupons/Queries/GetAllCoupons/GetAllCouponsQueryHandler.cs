using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Coupons.Queries.GetAllCoupons
{
    public class GetAllCouponsQueryHandler : IRequestHandler<GetAllCouponsQuery, Result<PaginatedResponse<IEnumerable<CouponResponse>>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllCouponsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<CouponResponse>>>> Handle(GetAllCouponsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Coupons.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(c => c.Code!.Contains(request.SearchTerm) || (c.Description != null && c.Description.Contains(request.SearchTerm)));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var coupons = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CouponResponse
                {
                    Id = c.Id,
                    Code = c.Code,
                    Description = c.Description,
                    DiscountType = c.DiscountType,
                    DiscountValue = c.DiscountValue,
                    MinOrderAmount = c.MinOrderAmount,
                    MaxDiscountAmount = c.MaxDiscountAmount,
                    UsageLimit = c.UsageLimit,
                    UsageCount = c.UsageCount,
                    ExpiresAt = c.ExpiresAt,
                    IsActive = c.IsActive,
                    IsFirstOrderOnly = c.IsFirstOrderOnly,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var paginationMeta = PaginationHelper.GetPaginationMetadata(request.PageNumber, request.PageSize, totalCount);
            paginationMeta.CurrentPageCount = coupons.Count;

            var response = new PaginatedResponse<IEnumerable<CouponResponse>>
            {
                Data = coupons,
                MetaData = paginationMeta
            };

            return Result<PaginatedResponse<IEnumerable<CouponResponse>>>.Ok(response, "Coupons retrieved successfully");
        }
    }
}

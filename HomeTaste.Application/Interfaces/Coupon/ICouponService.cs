using HomeTaste.Application.DTOs.Coupon;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.Coupon
{
    public interface ICouponService
    {
        Task<Result<PaginatedResponse<IEnumerable<CouponResponse>>>> GetAllCouponsAsync(int pageNumber = 1, int pageSize = 10, string searchTerm = null!);
        Task<Result<CouponResponse>> GetCouponByIdAsync(Guid id);
        Task<Result<CouponResponse>> CreateCouponAsync(CouponRequest request);
        Task<Result<CouponResponse>> UpdateCouponAsync(Guid id, CouponRequest request);
        Task<Result<bool>> DeleteCouponAsync(Guid id);
        Task<Result<bool>> ToggleActiveAsync(Guid id);
        Task<Result<CouponValidationResponse>> ValidateCouponAsync(ValidateCouponRequest request);
    }
}

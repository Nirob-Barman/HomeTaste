using HomeTaste.Application.DTOs.Coupon;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Validators.Coupon;
using HomeTaste.Application.Interfaces.Coupon;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.Coupon;
using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Services.Coupon
{
    public class CouponService : ICouponService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CouponService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<CouponResponse>>>> GetAllCouponsAsync(int pageNumber = 1, int pageSize = 10, string searchTerm = null!)
        {
            var query = _unitOfWork.Repository<Domain.Entities.Coupon.Coupon>().GetAllAsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(c => c.Code!.Contains(searchTerm) || (c.Description != null && c.Description.Contains(searchTerm)));

            var totalCount = await _unitOfWork.Repository<Domain.Entities.Coupon.Coupon>().CountAsync(query);

            var paged = _unitOfWork.Repository<Domain.Entities.Coupon.Coupon>().PaginateAsQueryable(query, pageNumber, pageSize);
            var coupons = await _unitOfWork.Repository<Domain.Entities.Coupon.Coupon>().ToEnumerableAsync(paged, c => MapToResponse(c));

            var meta = PaginationHelper.GetPaginationMetadata(pageNumber, pageSize, totalCount);
            var result = new PaginatedResponse<IEnumerable<CouponResponse>> { Data = coupons, MetaData = meta };

            return Result<PaginatedResponse<IEnumerable<CouponResponse>>>.Ok(result, "Coupons retrieved successfully", ResultType.Success);
        }

        public async Task<Result<CouponResponse>> GetCouponByIdAsync(Guid id)
        {
            var coupon = await _unitOfWork.Repository<Domain.Entities.Coupon.Coupon>().GetByIdAsync(id);
            if (coupon == null)
                return Result<CouponResponse>.Fail("Coupon not found.", "Not found", ResultType.NotFound);

            return Result<CouponResponse>.Ok(MapToResponse(coupon), "Coupon retrieved successfully", ResultType.Success);
        }

        public async Task<Result<CouponResponse>> CreateCouponAsync(CouponRequest request)
        {
            var errors = CouponRequestValidator.Validate(request);
            if (errors.Count > 0)
                return Result<CouponResponse>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            var code = request.Code?.Trim().ToUpperInvariant();
            var exists = await _unitOfWork.Repository<Domain.Entities.Coupon.Coupon>().AnyAsync(c => c.Code == code);
            if (exists)
                return Result<CouponResponse>.Fail("A coupon with this code already exists.", "Conflict", ResultType.Conflict);

            var coupon = new Domain.Entities.Coupon.Coupon
            {
                Id = Guid.NewGuid(),
                Code = code,
                Description = request.Description,
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                MinOrderAmount = request.MinOrderAmount,
                MaxDiscountAmount = request.MaxDiscountAmount,
                UsageLimit = request.UsageLimit,
                UsageCount = 0,
                ExpiresAt = request.ExpiresAt,
                IsActive = request.IsActive,
                IsFirstOrderOnly = request.IsFirstOrderOnly,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Domain.Entities.Coupon.Coupon>().AddAsync(coupon);
            await _unitOfWork.SaveChangesAsync();

            return Result<CouponResponse>.Ok(MapToResponse(coupon), "Coupon created successfully", ResultType.Created);
        }

        public async Task<Result<CouponResponse>> UpdateCouponAsync(Guid id, CouponRequest request)
        {
            var errors = CouponRequestValidator.Validate(request);
            if (errors.Count > 0)
                return Result<CouponResponse>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            var coupon = await _unitOfWork.Repository<Domain.Entities.Coupon.Coupon>().GetByIdAsync(id);
            if (coupon == null)
                return Result<CouponResponse>.Fail("Coupon not found.", "Not found", ResultType.NotFound);

            var code = request.Code?.Trim().ToUpperInvariant();
            var duplicate = await _unitOfWork.Repository<Domain.Entities.Coupon.Coupon>()
                .AnyAsync(c => c.Code == code && c.Id != id);
            if (duplicate)
                return Result<CouponResponse>.Fail("A coupon with this code already exists.", "Conflict", ResultType.Conflict);

            coupon.Code = code;
            coupon.Description = request.Description;
            coupon.DiscountType = request.DiscountType;
            coupon.DiscountValue = request.DiscountValue;
            coupon.MinOrderAmount = request.MinOrderAmount;
            coupon.MaxDiscountAmount = request.MaxDiscountAmount;
            coupon.UsageLimit = request.UsageLimit;
            coupon.ExpiresAt = request.ExpiresAt;
            coupon.IsActive = request.IsActive;
            coupon.IsFirstOrderOnly = request.IsFirstOrderOnly;
            coupon.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Domain.Entities.Coupon.Coupon>().Update(coupon);
            await _unitOfWork.SaveChangesAsync();

            return Result<CouponResponse>.Ok(MapToResponse(coupon), "Coupon updated successfully", ResultType.Success);
        }

        public async Task<Result<bool>> DeleteCouponAsync(Guid id)
        {
            var coupon = await _unitOfWork.Repository<Domain.Entities.Coupon.Coupon>().GetByIdAsync(id);
            if (coupon == null)
                return Result<bool>.Fail("Coupon not found.", "Not found", ResultType.NotFound);

            _unitOfWork.Repository<Domain.Entities.Coupon.Coupon>().Remove(coupon);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true, "Coupon deleted successfully", ResultType.Success);
        }

        public async Task<Result<bool>> ToggleActiveAsync(Guid id)
        {
            var coupon = await _unitOfWork.Repository<Domain.Entities.Coupon.Coupon>().GetByIdAsync(id);
            if (coupon == null)
                return Result<bool>.Fail("Coupon not found.", "Not found", ResultType.NotFound);

            coupon.IsActive = !coupon.IsActive;
            coupon.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Domain.Entities.Coupon.Coupon>().Update(coupon);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(coupon.IsActive, $"Coupon is now {(coupon.IsActive ? "active" : "inactive")}", ResultType.Success);
        }

        public async Task<Result<CouponValidationResponse>> ValidateCouponAsync(ValidateCouponRequest request)
        {
            var errors = ValidateCouponRequestValidator.Validate(request);
            if (errors.Count > 0)
                return Result<CouponValidationResponse>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            var code = request.Code?.Trim().ToUpperInvariant();
            var coupon = await _unitOfWork.Repository<Domain.Entities.Coupon.Coupon>()
                .FirstOrDefaultAsync(c => c.Code == code);

            if (coupon == null)
                return Result<CouponValidationResponse>.Ok(new CouponValidationResponse { IsValid = false, Message = "Coupon not found." }, "Validation complete", ResultType.Success);

            if (!coupon.IsActive)
                return Result<CouponValidationResponse>.Ok(new CouponValidationResponse { IsValid = false, Message = "Coupon is inactive." }, "Validation complete", ResultType.Success);

            if (coupon.ExpiresAt.HasValue && coupon.ExpiresAt.Value < DateTime.UtcNow)
                return Result<CouponValidationResponse>.Ok(new CouponValidationResponse { IsValid = false, Message = "Coupon has expired." }, "Validation complete", ResultType.Success);

            if (coupon.UsageLimit.HasValue && coupon.UsageCount >= coupon.UsageLimit.Value)
                return Result<CouponValidationResponse>.Ok(new CouponValidationResponse { IsValid = false, Message = "Coupon usage limit reached." }, "Validation complete", ResultType.Success);

            if (coupon.MinOrderAmount.HasValue && request.OrderAmount < coupon.MinOrderAmount.Value)
                return Result<CouponValidationResponse>.Ok(new CouponValidationResponse { IsValid = false, Message = $"Minimum order amount of {coupon.MinOrderAmount:C} required." }, "Validation complete", ResultType.Success);

            var discountAmount = coupon.DiscountType == DiscountType.Percentage
                ? request.OrderAmount * (coupon.DiscountValue / 100m)
                : coupon.DiscountValue;

            if (coupon.MaxDiscountAmount.HasValue && discountAmount > coupon.MaxDiscountAmount.Value)
                discountAmount = coupon.MaxDiscountAmount.Value;

            discountAmount = Math.Min(discountAmount, request.OrderAmount);

            return Result<CouponValidationResponse>.Ok(new CouponValidationResponse
            {
                IsValid = true,
                DiscountAmount = Math.Round(discountAmount, 2),
                Message = "Coupon applied successfully.",
                Coupon = MapToResponse(coupon)
            }, "Validation complete", ResultType.Success);
        }

        private static CouponResponse MapToResponse(Domain.Entities.Coupon.Coupon coupon) => new()
        {
            Id = coupon.Id,
            Code = coupon.Code,
            Description = coupon.Description,
            DiscountType = coupon.DiscountType,
            DiscountValue = coupon.DiscountValue,
            MinOrderAmount = coupon.MinOrderAmount,
            MaxDiscountAmount = coupon.MaxDiscountAmount,
            UsageLimit = coupon.UsageLimit,
            UsageCount = coupon.UsageCount,
            ExpiresAt = coupon.ExpiresAt,
            IsActive = coupon.IsActive,
            IsFirstOrderOnly = coupon.IsFirstOrderOnly,
            CreatedAt = coupon.CreatedAt
        };
    }
}

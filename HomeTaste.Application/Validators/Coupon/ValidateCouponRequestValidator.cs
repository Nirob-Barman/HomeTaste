using HomeTaste.Application.DTOs.Coupon;

namespace HomeTaste.Application.Validators.Coupon
{
    public static class ValidateCouponRequestValidator
    {
        public static List<string> Validate(ValidateCouponRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Code))
                errors.Add("Coupon code is required.");

            if (request.OrderAmount <= 0)
                errors.Add("Order amount must be greater than zero.");

            return errors;
        }
    }
}

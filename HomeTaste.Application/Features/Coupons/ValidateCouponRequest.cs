namespace HomeTaste.Application.Features.Coupons
{
    public class ValidateCouponRequest
    {
        public string? Code { get; set; }
        public decimal OrderAmount { get; set; }
    }
}

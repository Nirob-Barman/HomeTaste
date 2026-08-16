namespace HomeTaste.Application.Features.Coupons
{
    public record ValidateCouponRequest
    {
        public string? Code { get; set; }
        public decimal OrderAmount { get; set; }
    }
}

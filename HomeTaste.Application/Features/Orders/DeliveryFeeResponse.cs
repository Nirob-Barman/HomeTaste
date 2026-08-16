namespace HomeTaste.Application.Features.Orders
{
    public record DeliveryFeeResponse
    {
        public decimal Fee { get; set; }
        public bool IsFree => Fee == 0;
        public string Label => Fee == 0 ? "Free" : $"${Fee:F2}";
        public decimal FreeThreshold { get; set; } = 50m;
    }
}

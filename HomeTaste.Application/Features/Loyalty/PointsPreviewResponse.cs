namespace HomeTaste.Application.Features.Loyalty
{
    public record PointsPreviewResponse
    {
        public int PointsToRedeem { get; set; }
        public decimal DiscountAmount { get; set; }
        public int RemainingPoints { get; set; }
    }
}

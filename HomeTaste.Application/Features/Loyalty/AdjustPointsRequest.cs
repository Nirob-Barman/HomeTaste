namespace HomeTaste.Application.Features.Loyalty
{
    public record AdjustPointsRequest
    {
        public string? UserId { get; set; }
        public int Points { get; set; }
        public string? Description { get; set; }
    }
}

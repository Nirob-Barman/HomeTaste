namespace HomeTaste.Application.Features.MealReviews
{
    public record UpdateReviewRequest
    {
        public int? Rating { get; set; }
        public string? Feedback { get; set; }
    }
}

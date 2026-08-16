namespace HomeTaste.Application.Features.MealReviews
{
    public record SubmitReviewRequest
    {
        public Guid MealId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public string? Feedback { get; set; }
    }
}

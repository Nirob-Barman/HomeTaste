namespace HomeTaste.Application.Features.MealReviews
{
    public record ReviewResponse
    {
        public Guid Id { get; set; }
        public Guid MealId { get; set; }
        public string? MealName { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public string? Feedback { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

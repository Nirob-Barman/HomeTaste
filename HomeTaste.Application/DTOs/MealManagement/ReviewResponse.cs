

namespace HomeTaste.Application.DTOs.MealManagement
{
    public class ReviewResponse
    {
        public Guid Id { get; set; }
        public Guid MealId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public string? Feedback { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class DetailedReviewResponse
    {
        public Guid Id { get; set; }
        public Guid MealId { get; set; }
        public Guid UserId { get; set; }
        public string? UserEmail { get; set; }
        public int Rating { get; set; }
        public string? Feedback { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

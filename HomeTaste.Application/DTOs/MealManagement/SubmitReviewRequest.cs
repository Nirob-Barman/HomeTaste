

namespace HomeTaste.Application.DTOs.MealManagement
{
    public class SubmitReviewRequest
    {
        public Guid MealId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public string? Feedback { get; set; }
    }
}


namespace HomeTaste.Domain.Entities.MealManagement
{
    public class MealReview : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid MealId { get; set; }   // The meal being reviewed
        public Guid UserId { get; set; }  // The user submitting the review
        public int Rating { get; set; }     // Rating (e.g., 1-5)
        public string? Feedback { get; set; }

        public Meal? Meal { get; set; }
    }
}

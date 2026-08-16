
namespace HomeTaste.Domain.Entities.MealManagement
{
    public class MealReview : BaseEntity
    {
        public Guid MealId { get; private set; }   // The meal being reviewed
        public Guid UserId { get; private set; }  // The user submitting the review
        public int Rating { get; private set; }     // Rating (e.g., 1-5)
        public string? Feedback { get; private set; }

        public Meal? Meal { get; set; }

        private MealReview() { } // EF Core

        public static MealReview Create(Guid mealId, Guid userId, int rating, string? feedback, DateTime createdAt)
        {
            return new MealReview
            {
                MealId = mealId,
                UserId = userId,
                Rating = rating,
                Feedback = feedback,
                CreatedAt = createdAt
            };
        }

        public void UpdateFeedback(string? feedback, int? rating)
        {
            Feedback = feedback ?? Feedback;
            Rating = rating ?? Rating;
        }
    }
}

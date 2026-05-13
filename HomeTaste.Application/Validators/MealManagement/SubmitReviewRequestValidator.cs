using HomeTaste.Application.DTOs.MealManagement;

namespace HomeTaste.Application.Validators.MealManagement
{
    public static class SubmitReviewRequestValidator
    {
        public static List<string> Validate(SubmitReviewRequest request)
        {
            var errors = new List<string>();

            if (request.MealId == Guid.Empty)
                errors.Add("MealId is required.");

            if (request.Rating < 1 || request.Rating > 5)
                errors.Add("Rating must be between 1 and 5.");

            if (request.Feedback?.Length > 1000)
                errors.Add("Feedback cannot exceed 1000 characters.");

            return errors;
        }
    }
}

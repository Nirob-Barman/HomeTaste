using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.MealManagement
{
    public interface IMealReviewService
    {
        Task<Result<DetailedReviewResponse>> GetReviewByIdAsync(Guid reviewId);
        Task<Result<string>> SubmitReviewAsync(SubmitReviewRequest request);
        Task<Result<IEnumerable<ReviewResponse>>> GetMealReviewsAsync(Guid mealId);
        Task<Result<string>> UpdateReviewAsync(Guid reviewId, UpdateReviewRequest request);
        Task<Result<string>> DeleteReviewAsync(Guid reviewId);
        Task<Result<IEnumerable<ReviewResponse>>> GetMyReviewsAsync();
        Task<Result<decimal>> GetAverageMealRatingAsync(Guid mealId);
    }
}

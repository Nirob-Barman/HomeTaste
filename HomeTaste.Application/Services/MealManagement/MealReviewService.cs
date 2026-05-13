using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Interfaces.MealManagement;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Interfaces.TimeManagement;
using HomeTaste.Application.Validators.MealManagement;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.MealManagement;

namespace HomeTaste.Application.Services.MealManagement
{
    public class MealReviewService : IMealReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeService _dateTimeService;
        private readonly IUserContextService _userContextService;
        private readonly IUserManager _userManager;

        public MealReviewService(IUnitOfWork unitOfWork, 
            IDateTimeService dateTimeService, 
            IUserContextService userContextService,
            IUserManager userManager
            )
        {
            _unitOfWork = unitOfWork;
            _dateTimeService = dateTimeService;
            _userContextService = userContextService;
            _userManager = userManager;
        }

        public async Task<Result<DetailedReviewResponse>> GetReviewByIdAsync(Guid reviewId)
        {
            var review = await _unitOfWork.Repository<MealReview>().GetByIdAsync(reviewId);
            if (review == null)
            {
                return Result<DetailedReviewResponse>.Fail("Review not found", "Review not found", ResultType.NotFound);
            }

            var user = await _userManager.FindByIdAsync(review.UserId.ToString());

            var reviewResponse = new DetailedReviewResponse
            {
                Id = review.Id,
                MealId = review.MealId,
                UserId = review.UserId,
                UserEmail = user?.Email ?? string.Empty,
                Rating = review.Rating,
                Feedback = review.Feedback,
                CreatedAt = review.CreatedAt,
            };

            return Result<DetailedReviewResponse>.Ok(reviewResponse, "Review fetched successfully", ResultType.Success);
        }

        public async Task<Result<string>> SubmitReviewAsync(SubmitReviewRequest request)
        {
            var errors = SubmitReviewRequestValidator.Validate(request);
            if (errors.Count > 0)
                return Result<string>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            var meal = await _unitOfWork.Repository<Meal>().GetByIdAsync(request.MealId);
            if (meal == null)
            {
                return Result<string>.Fail("Meal not found", "Meal not found", ResultType.NotFound);
            }

            var review = new MealReview
            {
                MealId = request.MealId,
                UserId = request.UserId,
                Rating = request.Rating,
                Feedback = request.Feedback,
                CreatedAt = _dateTimeService.GetLocalNow(),
            };

            await _unitOfWork.Repository<MealReview>().AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Ok("Review submitted successfully", "Review submitted successfully", ResultType.Success);
        }

        public async Task<Result<IEnumerable<ReviewResponse>>> GetMealReviewsAsync(Guid mealId)
        {
            var meal = await _unitOfWork.Repository<Meal>().GetByIdAsync(mealId);
            if (meal == null)
            {
                return Result<IEnumerable<ReviewResponse>>.Fail("Meal not found", "Meal not found", ResultType.NotFound);
            }

            var reviews = await _unitOfWork.Repository<MealReview>().Where(r => r.MealId == mealId, r => new ReviewResponse
            {
                Id = r.Id,
                MealId = r.MealId,
                UserId = r.UserId,
                Rating = r.Rating,
                Feedback = r.Feedback,
                CreatedAt = r.CreatedAt,
            });

            return Result<IEnumerable<ReviewResponse>>.Ok(reviews, "Reviews fetched successfully", ResultType.Success);
        }

        public async Task<Result<string>> UpdateReviewAsync(Guid reviewId, UpdateReviewRequest request)
        {
            string userIdString = _userContextService.UserId!;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Result<string>.Fail("Invalid User ID.", "Invalid User ID", ResultType.Unauthorized);
            }
            var review = await _unitOfWork.Repository<MealReview>().GetByIdAsync(reviewId);
            if (review == null)
            {
                return Result<string>.Fail("Review not found.", "Review not found", ResultType.NotFound);
            }

            // Ensure the user is either the owner of the review or an admin
            if (review.UserId != userId && !_userContextService.IsInRole("Admin"))
            {
                return Result<string>.Fail("You are not authorized to update this review.", "Unauthorized", ResultType.Unauthorized);
            }

            // Update the review's feedback and rating
            review.Feedback = request.Feedback ?? review.Feedback;  // If no feedback is provided, keep the existing one
            review.Rating = request.Rating != null ? request.Rating.Value : review.Rating;  // Update only if the rating is provided

            _unitOfWork.Repository<MealReview>().Update(review);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Ok("Review updated successfully", "Review updated successfully", ResultType.Success);
        }

        public async Task<Result<string>> DeleteReviewAsync(Guid reviewId)
        {
            string userIdString = _userContextService.UserId!;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Result<string>.Fail("Invalid User ID.", "Invalid User ID", ResultType.Unauthorized);
            }

            var review = await _unitOfWork.Repository<MealReview>().GetByIdAsync(reviewId);
            if (review == null)
            {
                return Result<string>.Fail("Review not found.", "Review not found", ResultType.NotFound);
            }

            // Ensure the user is either the owner of the review or an admin
            if (review.UserId != userId && !_userContextService.IsInRole("Admin"))
            {
                return Result<string>.Fail("You are not authorized to delete this review.", "Unauthorized", ResultType.Unauthorized);
            }

            _unitOfWork.Repository<MealReview>().Remove(review);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Ok("Review deleted successfully", "Review deleted successfully", ResultType.Success);
        }

        public async Task<Result<IEnumerable<ReviewResponse>>> GetMyReviewsAsync()
        {
            string userIdString = _userContextService.UserId!;
            //string userIdString = "2b03f37a-fec4-4656-a2db-80d48fd1dcd3";
            //Guid userId = Guid.Parse(userIdString);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Result<IEnumerable<ReviewResponse>>.Fail("Invalid User ID.", "Invalid User ID", ResultType.Unauthorized);
            }

            var reviews = await _unitOfWork.Repository<MealReview>().Where(r => r.UserId == userId, r => new ReviewResponse
            {
                Id = r.Id,
                MealId = r.MealId,
                UserId = r.UserId,
                Rating = r.Rating,
                Feedback = r.Feedback,
                CreatedAt = r.CreatedAt
            });

            if (reviews == null || !reviews.Any())
            {
                return Result<IEnumerable<ReviewResponse>>.Fail("No reviews found for this user.", "No reviews found", ResultType.NotFound);
            }

            return Result<IEnumerable<ReviewResponse>>.Ok(reviews, "Reviews fetched successfully", ResultType.Success);
        }


        public async Task<Result<decimal>> GetAverageMealRatingAsync(Guid mealId)
        {
            // Fetch all reviews for the specific meal
            var reviews = await _unitOfWork.Repository<MealReview>().Where(r => r.MealId == mealId);

            if (reviews == null || !reviews.Any())
            {
                return Result<decimal>.Fail("No reviews found for this meal.", "No reviews found", ResultType.NotFound);
            }

            // Calculate the average rating
            var averageRating = (decimal)reviews.Average(r => r.Rating);

            return Result<decimal>.Ok(averageRating, "Average rating fetched successfully", ResultType.Success);
        }

    }
}
